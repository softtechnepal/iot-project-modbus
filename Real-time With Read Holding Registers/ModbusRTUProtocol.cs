﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Real_time_With_Read_Holding_Registers
{
    public class ModbusRTUProtocol
    {
        private byte slaveAddress = 1;
        private byte function = 3;
        private ushort startAddress = 40001;
        private uint _NumberOfPoints = 20;

        private SerialPort serialPort1 = null;
        private List<Register> _Registers = new List<Register>();
        private List<RegisterCopy> _RegistersCopy = new List<RegisterCopy>();
        private List<RegisterValue> _RegistersValue = new List<RegisterValue>();

        public ModbusRTUProtocol(uint numberOfPoints)
        {
            this._NumberOfPoints = numberOfPoints;
            for (ushort i = 0; i < NumberOfPoints; i++)
            {
                Registers.Add(new Register() { Address = (ushort)(startAddress + i) });
                RegistersCopy.Add(new RegisterCopy() { Address = (ushort)(startAddress + i) });
                RegistersValue.Add(new RegisterValue() { Address = (ushort)(startAddress + i) });
            }
        }

        public void Start()
        {
            try
            {
                serialPort1 = new SerialPort("COM23", 9600, Parity.None, 8, StopBits.One);
                serialPort1.Open();
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    while (true)
                    {
                        if (serialPort1.IsOpen)
                        {
                            byte[] frame = ReadHoldingRegistersMsg(slaveAddress, 0, function, NumberOfPoints);
                            serialPort1.Write(frame, 0, frame.Length);
                            Thread.Sleep(100); // Delay 100ms
                            if (serialPort1.BytesToRead >= 5)
                            {
                                byte[] bufferReceiver = new byte[this.serialPort1.BytesToRead];
                                serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                                serialPort1.DiscardInBuffer();

                                // Process data.
                                byte[] data = new byte[bufferReceiver.Length - 5];
                                Array.Copy(bufferReceiver, 3, data, 0, data.Length);

                                UInt16[] result = Word.ByteToUInt16(data);


                                byte[] name = new byte[bufferReceiver.Length - 5];
                                string[] binary = Word.ByteToString(data);
                                string[] binaryWithValue = Word.BinaryValue(data);
                                List<string> vals = new List<string>();
                                for (int i = 0; i < result.Length; i++)
                                {
                                    try
                                    {
                                        vals.Clear();
                                        foreach(char num in binaryWithValue[i])
                                        {
                                            if (num == '0')
                                            {
                                                vals.Add("Manual");
                                            }
                                            else
                                            {
                                                vals.Add("Auto");
                                            }
                                        }
                                        RegistersCopy[i].Value1 = binaryWithValue[i];
                                        RegistersValue[i].Value0 = vals[0];
                                        RegistersValue[i].Value1 = vals[1];
                                        RegistersValue[i].Value2 = vals[2];
                                        RegistersValue[i].Value3 = vals[3];
                                        RegistersValue[i].Value4 = vals[4];
                                        RegistersValue[i].Value5 = vals[5];
                                        RegistersValue[i].Value6 = vals[6];
                                        RegistersValue[i].Value7 = vals[7];
                                        RegistersValue[i].Value8 = vals[8];
                                        RegistersValue[i].Value9 = vals[9];
                                        RegistersValue[i].Value10 = vals[10];
                                        RegistersValue[i].Value11 = vals[11];
                                        RegistersValue[i].Value12 = vals[12];
                                        RegistersValue[i].Value13 = vals[13];
                                        RegistersValue[i].Value14 = vals[14];
                                        RegistersValue[i].Value15 = vals[15];
                                        
                                        RegistersCopy[i].Value1 = binary[i];
                                        Registers[i].Value = result[i];
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("From Here " + ex.Message);
                                    }
                                }
                            }
                        }
                        Thread.Sleep(100); // Delay 100ms
                    }
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /*foreach(char num in binary[i])
                                        {
                                            if (num == '0')
                                            {
                                                RegistersValue[i].Value0 = "Manual";
                                                RegistersValue[i].Value1= "Manual";
                                                RegistersValue[i].Value2 = "Manual";
                                                RegistersValue[i].Value3 = "Manual";
                                                RegistersValue[i].Value4 = "Manual";
                                                RegistersValue[i].Value5 = "Manual";
                                                RegistersValue[i].Value6 = "Manual";
                                                RegistersValue[i].Value7 = "Manual";
                                                RegistersValue[i].Value8 = "Manual";
                                                RegistersValue[i].Value9 = "Manual";
                                                RegistersValue[i].Value10 = "Manual";
                                                RegistersValue[i].Value11 = "Manual";
                                                RegistersValue[i].Value12 = "Manual";
                                                RegistersValue[i].Value13 = "Manual";
                                                RegistersValue[i].Value14 = "Manual";
                                                RegistersValue[i].Value15 = "Manual";

                                            }
                                            else
                                            {
                                                RegistersValue[i].Value0 = "Auto";
                                                RegistersValue[i].Value1 = "Auto";
                                                RegistersValue[i].Value2 = "Auto";
                                                RegistersValue[i].Value3 = "Auto";
                                                RegistersValue[i].Value4 = "Auto";
                                                RegistersValue[i].Value5 = "Auto";
                                                RegistersValue[i].Value6 = "Auto";
                                                RegistersValue[i].Value7 = "Auto";
                                                RegistersValue[i].Value8 = "Auto";
                                                RegistersValue[i].Value9 = "Auto";
                                                RegistersValue[i].Value10 = "Auto";
                                                RegistersValue[i].Value11 = "Auto";
                                                RegistersValue[i].Value12 = "Auto";
                                                RegistersValue[i].Value13 = "Auto";
                                                RegistersValue[i].Value14 = "Auto";
                                                RegistersValue[i].Value15 = "Auto";
                                            }
                                        }*/


        // RegistersCopy[i].Value1 = Word.DecToBinary(result[1]).ToString();
        /// <summary>
        /// Function 03 (03hex) Read Holding Registers
        /// Read the binary contents of holding registers in the slave.
        /// </summary>
        /// <param name="slaveAddress">Slave Address</param>
        /// <param name="startAddress">Starting Address</param>
        /// <param name="function">Function</param>
        /// <param name="numberOfPoints">Quantity of inputs</param>
        /// <returns>Byte Array</returns>
        /// unint
        private byte[] ReadHoldingRegistersMsg(byte slaveAddress, ushort startAddress, byte function, uint numberOfPoints)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveAddress;			    // Slave Address
            frame[1] = function;				    // Function             
            frame[2] = (byte)(startAddress >> 8);	// Starting Address High
            frame[3] = (byte)startAddress;		    // Starting Address Low            
            frame[4] = (byte)(numberOfPoints >> 8);	// Quantity of Registers High
            frame[5] = (byte)numberOfPoints;		// Quantity of Registers Low
            byte[] crc = this.CalculateCRC(frame);  // Calculate CRC.
            frame[frame.Length - 2] = crc[0];       // Error Check Low
            frame[frame.Length - 1] = crc[1];       // Error Check High
            return frame;
        }


        /// <summary>
        /// CRC Calculation 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] CalculateCRC(byte[] data)
        {
            ushort CRCFull = 0xFFFF; // Set the 16-bit register (CRC register) = FFFFH.
            char CRCLSB;
            byte[] CRC = new byte[2];
            for (int i = 0; i < (data.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ data[i]); // 

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = (byte)(CRCFull & 0xFF);
            return CRC;
        }

        public List<Register> Registers
        {
            get
            {
                return _Registers;
            }

            set
            {
                _Registers = value;
            }
        }

        public List<RegisterCopy> RegistersCopy
        {
            get
            {
                return _RegistersCopy;
            }

            set
            {
                _RegistersCopy = value;
            }
        }
        public List<RegisterValue> RegistersValue
        {
            get
            {
                return _RegistersValue;
            }

            set
            {
                _RegistersValue = value;
            }
        }

        public uint NumberOfPoints
        {
            get
            {
                return _NumberOfPoints;
            }

            set
            {
                _NumberOfPoints = value;
            }
        }
    }
    /*RegistersCopy[0].Value1 = final_result[0];
                                        RegistersCopy[1].Value2 = final_result[1];*/
    /*if (i == 1)
    {
        *//*List<string> final_result = Word.DecToString(result[i]);
        foreach (string value in final_result)
        {
            RegistersCopy[i].Value1 = value;
        }*//*
        List<string> final_result = Word.DecToString(result[i]);
        foreach (string inum in final_result)
        {
            RegistersCopy[0].Value1 = inum;//final_result[0];
            RegistersCopy[1].Value2 = final_result[1];
        }

    }*/
    /*if (i == 2)
    {
        List<string> final_result = Word.DecToString(result[i]);
        foreach (string value in final_result)
        {
            RegistersCopy[i].Value2 = value;
        }
    }*/
}
