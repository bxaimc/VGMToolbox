using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class CriField
    {        
        public byte Type { set; get; }
        public string Name { set; get; }        
        public object Value { set; get; }
    }
    
    public class CriUtfTable
    {
        // thanks to hcs for all of this!!!

        #region Constants

        public static readonly byte[] SIGNATURE_BYTES = new byte[] { 0x40, 0x55, 0x54, 0x46 };

        public const byte COLUMN_STORAGE_MASK = 0xF0;
        public const byte COLUMN_STORAGE_PERROW = 0x50;
        public const byte COLUMN_STORAGE_CONSTANT = 0x30;
        public const byte COLUMN_STORAGE_ZERO = 0x10;

        // I suspect that "type 2" is signed
        public const byte COLUMN_TYPE_MASK = 0x0F;
        public const byte COLUMN_TYPE_DATA = 0x0B;
        public const byte COLUMN_TYPE_STRING = 0x0A;
        // 0x09 double? 
        public const byte COLUMN_TYPE_FLOAT = 0x08;
        // 0x07 signed 8byte? 
        public const byte COLUMN_TYPE_8BYTE = 0x06;
        public const byte COLUMN_TYPE_4BYTE2 = 0x05;
        public const byte COLUMN_TYPE_4BYTE = 0x04;
        public const byte COLUMN_TYPE_2BYTE2 = 0x03;
        public const byte COLUMN_TYPE_2BYTE = 0x02;
        public const byte COLUMN_TYPE_1BYTE2 = 0x01;
        public const byte COLUMN_TYPE_1BYTE = 0x00;

        #endregion

        #region Members

        public long BaseOffset { set; get; }

        public byte[] MagicBytes { set; get; }
        public uint TableSize { set; get; }

        public uint RowOffset { set; get; }
        public uint StringTableOffset { set; get; }
        public uint DataOffset { set; get; }

        public uint TableNameOffset { set; get; }
        public string TableName { set; get; }

        public ushort NumberOfFields { set; get; }
        
        public ushort RowSize { set; get; }
        public uint NumberOfRows { set; get; }

        public Dictionary<string, CriField>[] Rows { set; get; }

        #endregion

        public void Initialize(FileStream fs, long offset)
        {
            this.MagicBytes = ParseFile.ParseSimpleOffset(fs, offset + 0, 4);

            if (ParseFile.CompareSegment(this.MagicBytes, 0, SIGNATURE_BYTES))
            {
                // set base offset
                this.BaseOffset = offset;

                // read header
                this.initializeUtfHeader(fs);

                // initialize rows
                this.Rows = new Dictionary<string, CriField>[this.NumberOfRows];

                // read schema
                this.initializeUtfHeader(fs);
                this.initializeUtfSchema(fs, (this.BaseOffset + 0x20));
            }
            else
            {
                throw new FormatException(String.Format("@UTF signature not found at offset <0x{0}>.", offset.ToString("X8")));           
            }
        
        }

        private void initializeUtfHeader(FileStream fs)
        {
            this.TableSize = ParseFile.ReadUintBE(fs, this.BaseOffset + 4);

            this.RowOffset = ParseFile.ReadUintBE(fs, this.BaseOffset + 8) + 8;
            this.StringTableOffset = ParseFile.ReadUintBE(fs, this.BaseOffset + 0xC) + 8;
            this.DataOffset = ParseFile.ReadUintBE(fs, this.BaseOffset + 0x10) + 8;

            this.TableNameOffset = ParseFile.ReadUintBE(fs, this.BaseOffset + 0x14);
            this.TableName = ParseFile.ReadAsciiString(fs, (this.BaseOffset + this.StringTableOffset + this.TableNameOffset));

            this.NumberOfFields = ParseFile.ReadUshortBE(fs, this.BaseOffset + 0x18);

            this.RowSize = ParseFile.ReadUshortBE(fs, this.BaseOffset + 0x1A);
            this.NumberOfRows = ParseFile.ReadUintBE(fs, this.BaseOffset + 0x1C);        
        }

        private void initializeUtfSchema(FileStream fs, long schemaOffset)
        {
            long nameOffset;

            long constantOffset;
            
            long dataOffset;
            long dataSize;

            long rowDataOffset;
            long rowDataSize;

            long currentOffset = schemaOffset;
            long currentRowBase;
            long currentRowOffset = 0;
            
            CriField field;

            for (uint i = 0; i < this.NumberOfRows; i++)
            {
                currentOffset = schemaOffset;
                currentRowBase = this.BaseOffset + this.RowOffset + (this.RowSize * i);
                this.Rows[i] = new Dictionary<string, CriField>();
                
                // parse fields
                for (ushort j = 0; j < this.NumberOfFields; j++)
                {
                    field = new CriField();

                    field.Type = ParseFile.ReadByte(fs, currentOffset);
                    nameOffset = ParseFile.ReadUintBE(fs, currentOffset + 1);
                    field.Name = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + nameOffset);

                    // each row will have a constant
                    if ((field.Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)
                    {
                        // capture offset of constant
                        constantOffset = currentOffset + 5;

                        // read the constant depending on the type
                        switch (field.Type & COLUMN_TYPE_MASK)
                        {
                            case COLUMN_TYPE_STRING:
                                dataOffset = ParseFile.ReadUintBE(fs, constantOffset);
                                field.Value = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + dataOffset);
                                currentOffset += 4;
                                break;
                            case COLUMN_TYPE_8BYTE:
                                field.Value = ParseFile.ReadUlongBE(fs, constantOffset);
                                currentOffset += 8;
                                break;
                            case COLUMN_TYPE_DATA:
                                dataOffset = ParseFile.ReadUintBE(fs, constantOffset);
                                dataSize = ParseFile.ReadUintBE(fs, constantOffset + 4);
                                field.Value = ParseFile.ParseSimpleOffset(fs, this.BaseOffset + this.DataOffset + dataOffset, (int)dataSize);
                                currentOffset += 8;
                                break;
                            case COLUMN_TYPE_FLOAT:
                                field.Value = ParseFile.ReadFloatBE(fs, constantOffset);
                                currentOffset += 4;
                                break;
                            case COLUMN_TYPE_4BYTE2:
                                field.Value = ParseFile.ReadInt32BE(fs, constantOffset);
                                currentOffset += 4;
                                break;
                            case COLUMN_TYPE_4BYTE:
                                field.Value = ParseFile.ReadUintBE(fs, constantOffset);
                                currentOffset += 4;
                                break;
                            case COLUMN_TYPE_2BYTE2:
                                field.Value = ParseFile.ReadInt16BE(fs, constantOffset);
                                currentOffset += 2;
                                break;
                            case COLUMN_TYPE_2BYTE:
                                field.Value = ParseFile.ReadUshortBE(fs, constantOffset);
                                currentOffset += 2;
                                break;
                            case COLUMN_TYPE_1BYTE2:
                                field.Value = ParseFile.ReadSByte(fs, constantOffset);
                                currentOffset += 1;
                                break;
                            case COLUMN_TYPE_1BYTE:
                                field.Value = ParseFile.ReadByte(fs, constantOffset);
                                currentOffset += 1;
                                break;
                            default:
                                throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));

                        } // switch (field.Type & COLUMN_TYPE_MASK)
                    }
                    else if ((field.Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_PERROW)
                    {
                        // read the constant depending on the type
                        switch (field.Type & COLUMN_TYPE_MASK)
                        {
                            case COLUMN_TYPE_STRING:
                                rowDataOffset = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset);
                                field.Value = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + rowDataOffset);
                                currentRowOffset += 4;
                                break;
                            case COLUMN_TYPE_8BYTE:
                                field.Value = ParseFile.ReadUlongBE(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 8;
                                break;
                            case COLUMN_TYPE_DATA:
                                rowDataOffset = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset);
                                rowDataSize = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset + 4);
                                field.Value = ParseFile.ParseSimpleOffset(fs, this.BaseOffset + this.DataOffset + rowDataOffset, (int)rowDataSize);
                                currentRowOffset += 8;
                                break;
                            case COLUMN_TYPE_FLOAT:
                                field.Value = ParseFile.ReadFloatBE(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 4;
                                break;
                            case COLUMN_TYPE_4BYTE2:
                                field.Value = ParseFile.ReadInt32BE(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 4;
                                break;
                            case COLUMN_TYPE_4BYTE:
                                field.Value = ParseFile.ReadUintBE(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 4;
                                break;
                            case COLUMN_TYPE_2BYTE2:
                                field.Value = ParseFile.ReadInt16BE(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 2;
                                break;
                            case COLUMN_TYPE_2BYTE:
                                field.Value = ParseFile.ReadUshortBE(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 2;
                                break;
                            case COLUMN_TYPE_1BYTE2:
                                field.Value = ParseFile.ReadSByte(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 1;
                                break;
                            case COLUMN_TYPE_1BYTE:
                                field.Value = ParseFile.ReadByte(fs, currentRowBase + currentRowOffset);
                                currentRowOffset += 1;
                                break;
                            default:
                                throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));

                        } // switch (field.Type & COLUMN_TYPE_MASK)

                    } // if ((fields[i].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)

                    // add field to dictionary
                    this.Rows[i].Add(field.Name, field);

                    // move to next field
                    currentOffset += 5; //  sizeof(CriField.Type + CriField.NameOffset)

                } // for (ushort j = 0; j < this.NumberOfFields; j++)
            } // for (uint i = 0; i < this.NumberOfRows; i++)
        }
    }
}
