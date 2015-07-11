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

        // public uint NameOffset { set; get; }
        public string Name { set; get; }
        
        // public long ConstantOffset { set; get; }

        // public long RowDataOffset { set; get; }
        // public byte RowDataSize { set; get; }

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

        public CriField[] Fields { set; get; }
        Dictionary<string, CriField> FieldDictionary;

        #endregion

        public void Initialize(FileStream fs, long offset)
        {
            this.MagicBytes = ParseFile.ParseSimpleOffset(fs, offset + 0, 4);

            if (ParseFile.CompareSegment(this.MagicBytes, 0, SIGNATURE_BYTES))
            {
                // set base offset
                this.BaseOffset = offset;

                // initialize dictionary
                this.FieldDictionary = new Dictionary<string, CriField>();

                // read header
                this.initializeUtfHeader(fs);

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
            long currentRowOffset = 0;


            this.Fields = new CriField[this.NumberOfFields];
            
            // parse fields
            for (ushort j = 0; j < this.NumberOfFields; j++)
            {
                this.Fields[j] = new CriField();

                this.Fields[j].Type = ParseFile.ReadByte(fs, currentOffset);
                // this.Fields[j].NameOffset = ParseFile.ReadUintBE(fs, currentOffset + 1);
                nameOffset = ParseFile.ReadUintBE(fs, currentOffset + 1);
                // this.Fields[j].Name = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + this.Fields[j].NameOffset);
                this.Fields[j].Name = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + nameOffset);

                // each row will have a constant
                if ((this.Fields[j].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)
                {
                    // capture offset of constant
                    // this.Fields[j].ConstantOffset = currentOffset + 5;
                    constantOffset = currentOffset + 5;

                    // read the constant depending on the type
                    switch (this.Fields[j].Type & COLUMN_TYPE_MASK)
                    {
                        case COLUMN_TYPE_STRING:
                            dataOffset = ParseFile.ReadUintBE(fs, constantOffset);
                            this.Fields[j].Value = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + dataOffset);
                            currentOffset += 4;
                            break;
                        case COLUMN_TYPE_8BYTE:
                            this.Fields[j].Value = ParseFile.ReadUlongBE(fs, constantOffset);
                            currentOffset += 8;
                            break;
                        case COLUMN_TYPE_DATA:
                            dataOffset = ParseFile.ReadUintBE(fs, constantOffset);
                            dataSize = ParseFile.ReadUintBE(fs, constantOffset + 4);
                            this.Fields[j].Value = ParseFile.ParseSimpleOffset(fs, this.BaseOffset + this.DataOffset + dataOffset, (int)dataSize);
                            currentOffset += 8;
                            break;
                        case COLUMN_TYPE_FLOAT:
                            this.Fields[j].Value = ParseFile.ReadFloatBE(fs, constantOffset);
                            currentOffset += 4;
                            break;
                        case COLUMN_TYPE_4BYTE2:
                            this.Fields[j].Value = ParseFile.ReadInt32BE(fs, constantOffset);
                            currentOffset += 4;
                            break;
                        case COLUMN_TYPE_4BYTE:
                            this.Fields[j].Value = ParseFile.ReadUintBE(fs, constantOffset);
                            currentOffset += 4;
                            break;
                        case COLUMN_TYPE_2BYTE2:
                            this.Fields[j].Value = ParseFile.ReadInt16BE(fs, constantOffset);
                            currentOffset += 2;
                            break;
                        case COLUMN_TYPE_2BYTE:
                            this.Fields[j].Value = ParseFile.ReadUshortBE(fs, constantOffset);
                            currentOffset += 2;
                            break;
                        case COLUMN_TYPE_1BYTE2:
                            this.Fields[j].Value = ParseFile.ReadSByte(fs, constantOffset);
                            currentOffset += 1;
                            break;
                        case COLUMN_TYPE_1BYTE:
                            this.Fields[j].Value = ParseFile.ReadByte(fs, constantOffset);
                            currentOffset += 1;
                            break;
                        default:
                            throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));
                   
                    } // switch (this.Fields[i].Type & COLUMN_TYPE_MASK)
                }
                else if ((this.Fields[j].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_PERROW)
                {
                    // this.Fields[j].RowDataOffset = this.RowOffset + currentRowOffset;
                    // rowDataOffset = this.RowOffset + currentRowOffset;

                    // read the constant depending on the type
                    switch (this.Fields[j].Type & COLUMN_TYPE_MASK)
                    {
                        case COLUMN_TYPE_STRING:
                            rowDataOffset = ParseFile.ReadUintBE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            this.Fields[j].Value = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + rowDataOffset);
                            currentRowOffset += 4;
                            break;
                        case COLUMN_TYPE_8BYTE:
                            this.Fields[j].Value = ParseFile.ReadUlongBE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 8;
                            break;
                        case COLUMN_TYPE_DATA:
                            rowDataOffset = ParseFile.ReadUintBE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            rowDataSize = ParseFile.ReadUintBE(fs, this.BaseOffset + this.RowOffset + currentRowOffset + 4);
                            this.Fields[j].Value = ParseFile.ParseSimpleOffset(fs, this.BaseOffset + this.DataOffset + rowDataOffset, (int)rowDataSize);
                            currentRowOffset += 8;
                            break;
                        case COLUMN_TYPE_FLOAT:
                            this.Fields[j].Value = ParseFile.ReadFloatBE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 4;
                            break;
                        case COLUMN_TYPE_4BYTE2:
                            this.Fields[j].Value = ParseFile.ReadInt32BE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 4;
                            break;
                        case COLUMN_TYPE_4BYTE:
                            this.Fields[j].Value = ParseFile.ReadUintBE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 4;
                            break;
                        case COLUMN_TYPE_2BYTE2:
                            this.Fields[j].Value = ParseFile.ReadInt16BE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 2;
                            break;
                        case COLUMN_TYPE_2BYTE:
                            this.Fields[j].Value = ParseFile.ReadUshortBE(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 2;
                            break;
                        case COLUMN_TYPE_1BYTE2:
                            this.Fields[j].Value = ParseFile.ReadSByte(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 1;
                            break;
                        case COLUMN_TYPE_1BYTE:
                            this.Fields[j].Value = ParseFile.ReadByte(fs, this.BaseOffset + this.RowOffset + currentRowOffset);
                            currentRowOffset += 1;
                            break;
                        default:
                            throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));

                    } // switch (this.Fields[i].Type & COLUMN_TYPE_MASK)

                    // currentRowOffset += this.Fields[j].RowDataSize;

                } // if ((this.Fields[i].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)
                
                // add field to dictionary
                this.FieldDictionary.Add(this.Fields[j].Name, this.Fields[j]);

                // move to next field
                currentOffset += 5; //  sizeof(CriField.Type + CriField.NameOffset)

            } // for (ushort i = 0; i < this.NumberOfFields; i++)
        
        }

    }
}
