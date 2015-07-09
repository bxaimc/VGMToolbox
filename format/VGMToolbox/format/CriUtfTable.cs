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
        public enum StorageType { RowStorage, Constant, Undefined };
        
        public byte Type { set; get; }
        
        public uint NameOffset { set; get; }
        public string Name { set; get; }
        
        public long ConstantOffset { set; get; }

        public long RowDataOffset { set; get; }
        public byte RowDataSize { set; get; }
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
            long currentOffset = schemaOffset;
            long currentRowOffset = 0;

            this.Fields = new CriField[this.NumberOfFields];
            
            // parse fields
            for (ushort i = 0; i < this.NumberOfFields; i++)
            {
                this.Fields[i] = new CriField();

                this.Fields[i].Type = ParseFile.ReadByte(fs, currentOffset);
                this.Fields[i].NameOffset = ParseFile.ReadUintBE(fs, currentOffset + 1);
                this.Fields[i].Name = ParseFile.ReadAsciiString(fs, this.BaseOffset + this.StringTableOffset + this.Fields[i].NameOffset);

                // each row will have a constant
                if ((this.Fields[i].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)
                {
                    // capture offset of constant
                    this.Fields[i].ConstantOffset = currentOffset + 5;
                    
                    // read the constant depending on the type
                    switch (this.Fields[i].Type & COLUMN_TYPE_MASK)
                    {
                        case COLUMN_TYPE_STRING:
                            currentOffset += 4;
                            break;
                        case COLUMN_TYPE_8BYTE:
                        case COLUMN_TYPE_DATA:
                            currentOffset += 8;
                            break;
                        case COLUMN_TYPE_FLOAT:
                        case COLUMN_TYPE_4BYTE2:
                        case COLUMN_TYPE_4BYTE:
                            currentOffset += 4;
                            break;
                        case COLUMN_TYPE_2BYTE2:
                        case COLUMN_TYPE_2BYTE:
                            currentOffset += 2;
                            break;
                        case COLUMN_TYPE_1BYTE2:
                        case COLUMN_TYPE_1BYTE:
                            currentOffset += 1;
                            break;
                        default:
                            throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));
                   
                    } // switch (this.Fields[i].Type & COLUMN_TYPE_MASK)
                }
                else if ((this.Fields[i].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_PERROW)
                {
                    this.Fields[i].RowDataOffset = this.RowOffset + currentRowOffset;

                    // read the constant depending on the type
                    switch (this.Fields[i].Type & COLUMN_TYPE_MASK)
                    {
                        case COLUMN_TYPE_STRING:
                            this.Fields[i].RowDataSize = 4;
                            break;
                        case COLUMN_TYPE_8BYTE:
                        case COLUMN_TYPE_DATA:
                            this.Fields[i].RowDataSize = 8;
                            break;
                        case COLUMN_TYPE_FLOAT:
                        case COLUMN_TYPE_4BYTE2:
                        case COLUMN_TYPE_4BYTE:
                            this.Fields[i].RowDataSize = 4;
                            break;
                        case COLUMN_TYPE_2BYTE2:
                        case COLUMN_TYPE_2BYTE:
                            this.Fields[i].RowDataSize = 2;
                            break;
                        case COLUMN_TYPE_1BYTE2:
                        case COLUMN_TYPE_1BYTE:
                            this.Fields[i].RowDataSize = 1;
                            break;
                        default:
                            throw new FormatException(String.Format("Unknown COLUMN TYPE at offset: 0x{0}", currentOffset.ToString("X8")));

                    } // switch (this.Fields[i].Type & COLUMN_TYPE_MASK)

                    currentRowOffset += this.Fields[i].RowDataSize;

                } // if ((this.Fields[i].Type & COLUMN_STORAGE_MASK) == COLUMN_STORAGE_CONSTANT)
                
                // move to next field
                currentOffset += 5; //  sizeof(CriField.Type + CriField.NameOffset)

            } // for (ushort i = 0; i < this.NumberOfFields; i++)
        
        }

    }
}
