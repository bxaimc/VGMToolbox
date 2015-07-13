using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class CriCpkArchive
    {
        public static readonly byte[] CPK_SIGNATURE = new byte[] { 0x43, 0x50, 0x4B, 0x20 };
        public static readonly byte[] CRILAYLA_SIGNATURE = new byte[] { 0x43, 0x52, 0x49, 0x4C, 0x41, 0x59, 0x4C, 0x41 };

        // public static byte[] CriCpkDecompressionOutputBuffer = new byte[64000000];






        private static ushort get_next_bits(Stream inFile, ref long offset, ref byte bit_pool, ref int bits_left, 
            int bit_count)
        {
            ushort out_bits = 0;
            int num_bits_produced = 0;
    
            while (num_bits_produced < bit_count)
            {
                if (bits_left == 0)
                {
                    bit_pool = ParseFile.ReadByte(inFile, offset);
                    bits_left = 8;
                    offset--;
                }

                int bits_this_round;

                if (bits_left > (bit_count - num_bits_produced))
                {
                    bits_this_round = bit_count - num_bits_produced;
                }
                else
                {
                    bits_this_round = bits_left;
                }

                out_bits <<= bits_this_round;
                out_bits |= (ushort)((ushort)(bit_pool >> (bits_left - bits_this_round)) & ((1 << bits_this_round) - 1));


                bits_left -= bits_this_round;
                num_bits_produced += bits_this_round;
            }

            return out_bits;
        }

        public static long Uncompress(Stream inFile, long offset, long input_size, string outFile)
        {
            byte[] output_buffer = null;
            long bytes_output = 0;
            byte[] magicBytes = ParseFile.ParseSimpleOffset(inFile, offset, CRILAYLA_SIGNATURE.GetLength(0));
            
            if (!ParseFile.CompareSegment(magicBytes, 0, CRILAYLA_SIGNATURE))
            {
                throw new FormatException(String.Format("CRILAYLA Signature not found at offset: 0x{0}", offset.ToString("X8")));
            }

            long uncompressed_size = (long)ParseFile.ReadUintLE(inFile, offset + 8);
            long uncompressed_header_offset = (long)(offset + ParseFile.ReadUintLE(inFile, offset + 0x0C) + 0x10);

            if ((uncompressed_header_offset + 0x100) != (offset + input_size))
            {
                throw new FormatException(String.Format("CRILAYLA: Uncompressed header size does not match expected size at offset: 0x{0}", offset.ToString("X8")));            
            }
            
            using (FileStream os = File.Open(outFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {                                
                using (BufferedStream outStream = new BufferedStream(os))
                {
                    // set length of output file
                    outStream.SetLength(uncompressed_size + 0x100);

                    // write uncompressed header
                    output_buffer = ParseFile.ParseSimpleOffset(inFile, uncompressed_header_offset, 0x100);
                    outStream.Write(output_buffer, 0, 0x100);

                    // do the hocus pocus?
                    long input_end = offset + input_size - 0x100 - 1;
                    long input_offset = input_end;
                    long output_end = 0x100 + uncompressed_size - 1;

                    byte bit_pool = 0;
                    int bits_left = 0;
                    int[] vle_lens = new int[4] { 2, 3, 5, 8 };

                    long backreference_offset;
                    long backreference_length;

                    int vle_level;
                    int this_level;

                    ushort temp;

                    while (bytes_output < uncompressed_size)
                    {
                        if (get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 1) != 0)
                        {
                            backreference_offset = output_end - bytes_output +
                                get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 13) + 3;
                            backreference_length = 3;

                            // decode variable length coding for length                        
                            for (vle_level = 0; vle_level < vle_lens.Length; vle_level++)
                            {
                                this_level = get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, vle_lens[vle_level]);
                                backreference_length += this_level;

                                if (this_level != ((1 << vle_lens[vle_level]) - 1))
                                {
                                    break;
                                }
                            }

                            if (vle_level == vle_lens.Length)
                            {
                                do
                                {
                                    this_level = get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 8);
                                    backreference_length += this_level;

                                } while (this_level == 255);
                            }

                            //printf("0x%08lx backreference to 0x%lx, length 0x%lx\n", output_end-bytes_output, backreference_offset, backreference_length);
                            for (int i = 0; i < backreference_length; i++)
                            {
                                //output_buffer[output_end - bytes_output] = output_buffer[backreference_offset--];

                                output_buffer = ParseFile.ParseSimpleOffset(outStream, backreference_offset--, 1);
                                outStream.Position = (output_end - bytes_output);
                                outStream.Write(output_buffer, 0, 1);

                                bytes_output++;
                            }
                        }
                        else
                        {
                            // verbatim byte
                            // output_buffer[output_end - bytes_output] = get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 8);

                            temp = get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 8);
                            output_buffer = BitConverter.GetBytes(temp);
                            outStream.Position = (output_end - bytes_output);
                            outStream.Write(output_buffer, 0, 1);

                            //printf("0x%08lx verbatim byte\n", output_end-bytes_output);
                            bytes_output++;
                        }
                    }
                } // using (BufferedStream outStream = new BufferedStream(os))            
            } // using (FileStream outStream = File.Open(outFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                      
            return 0x100 + bytes_output;
        }

        public static long Uncompress2(Stream inFile, long offset, long input_size, string outFile)
        {
            byte[] output_buffer;            
            long bytes_output = 0;
            byte[] magicBytes = ParseFile.ParseSimpleOffset(inFile, offset, CRILAYLA_SIGNATURE.GetLength(0));

            if (!ParseFile.CompareSegment(magicBytes, 0, CRILAYLA_SIGNATURE))
            {
                throw new FormatException(String.Format("CRILAYLA Signature not found at offset: 0x{0}", offset.ToString("X8")));
            }

            long uncompressed_size = (long)ParseFile.ReadUintLE(inFile, offset + 8);
            long uncompressed_header_offset = (long)(offset + ParseFile.ReadUintLE(inFile, offset + 0x0C) + 0x10);
           
            if ((uncompressed_header_offset + 0x100) != (offset + input_size))
            {
                throw new FormatException(String.Format("CRILAYLA: Uncompressed header size does not match expected size at offset: 0x{0}", offset.ToString("X8")));
            }

            output_buffer = new byte[uncompressed_size + 0x100];

            // write uncompressed header
            inFile.Position = uncompressed_header_offset;
            inFile.Read(output_buffer, 0, 0x100);

            // do the hocus pocus?
            long input_end = offset + input_size - 0x100 - 1;
            long input_offset = input_end;
            long output_end = 0x100 + uncompressed_size - 1;

            byte bit_pool = 0;
            int bits_left = 0;
            int[] vle_lens = new int[4] { 2, 3, 5, 8 };

            long backreference_offset;
            long backreference_length;

            int vle_level;
            int this_level;

            ushort temp;

            while (bytes_output < uncompressed_size)
            {
                if (get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 1) > 0)
                {
                    backreference_offset = output_end - bytes_output +
                        get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 13) + 3;
                    backreference_length = 3;

                    // decode variable length coding for length                        
                    for (vle_level = 0; vle_level < vle_lens.Length; vle_level++)
                    {
                        this_level = get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, vle_lens[vle_level]);
                        backreference_length += this_level;

                        if (this_level != ((1 << vle_lens[vle_level]) - 1))
                        {
                            break;
                        }
                    }

                    if (vle_level == vle_lens.Length)
                    {
                        do
                        {
                            this_level = get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 8);
                            backreference_length += this_level;

                        } while (this_level == 255);
                    }

                    //printf("0x%08lx backreference to 0x%lx, length 0x%lx\n", output_end-bytes_output, backreference_offset, backreference_length);
                    for (int i = 0; i < backreference_length; i++)
                    {
                        output_buffer[output_end - bytes_output] = output_buffer[backreference_offset--];
                        bytes_output++;
                    }
                }
                else
                {
                    // verbatim byte
                    temp = get_next_bits(inFile, ref input_offset, ref bit_pool, ref bits_left, 8);
                    output_buffer[output_end - bytes_output] = (byte)temp;

                    //printf("0x%08lx verbatim byte\n", output_end-bytes_output);
                    bytes_output++;
                }
            }

            using (FileStream outStream = File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                outStream.Write(output_buffer, 0, output_buffer.Length);
            }

            return 0x100 + bytes_output;
        }
    }
}
