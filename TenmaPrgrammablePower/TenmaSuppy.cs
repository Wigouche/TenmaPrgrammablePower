using System;
using System.Threading;

using System.IO.Ports;
//note the power supply doesnt except commands strung together and a wait time is needed between successive commands to make sure it opperates correctly
namespace TenmaPrgrammablePower
{
    //todo implement data rx timeout as currently relies on fixd rx values or waits forever

    public class TenmaSupply : IDisposable
    {
        private SerialPort ComPort;

        public TenmaSupply(string ComPortName)
        {
            //todo validate com portname before making port

            ComPort = new SerialPort(ComPortName)
            {
                BaudRate = 9600,
                StopBits = StopBits.One,
                DataBits = 8,
                Parity = Parity.None,
                ReadTimeout = 2000,
                WriteTimeout = 2000,
            };

            ComPort.Open();
            ComPort.DiscardInBuffer();
        }

        public void Dispose()
        {
            if (ComPort.IsOpen)
                ComPort.Close();
            ComPort.Dispose();
        }

        public void SetV(decimal newVoltage)
        {
            if (newVoltage >= 0 && newVoltage <= 60)
            {
                ComPort.Write(String.Format("VSET1:{0:00.00}", newVoltage));
            }
            else
            {
                throw (new ArgumentException(string.Format("input {0} is out of range for supply. must bwe between 0 and 60V", newVoltage)));
            }
        }

        public void SetI(decimal newCurrent)
        {
            if (newCurrent >= 0 && newCurrent <= 3)
            {
                ComPort.Write(string.Format("ISET1:{0:0.000}", newCurrent));
            }
            else
            {
                throw (new ArgumentException(string.Format("input {0} is out of range for supply. must bwe between 0 and 3A",newCurrent)));
            }
        }

        private decimal GetAndConvertResponce()
        {
            while (ComPort.BytesToRead < 5) ;
            char[] responce = new char[5];
            ComPort.Read(responce,0,5);
            if (decimal.TryParse(new string(responce,0,5), out decimal result))
            {
                return result;
            }
            else
            {
                throw (new ArgumentException("invald responce from supply cannot prase to decimal"));
            }
        }

        public decimal GetVSet()
        {
            ComPort.DiscardInBuffer();
            ComPort.Write("VSET1?");
            return GetAndConvertResponce();
        }

        public decimal GetISet()
        {
            ComPort.DiscardInBuffer();
            ComPort.Write("ISET1?");
            return GetAndConvertResponce();
        }

        public decimal GetVOut()
        {
            ComPort.DiscardInBuffer();
            ComPort.Write("VOUT1?");
            return GetAndConvertResponce();
        }

        public decimal GetIOut()
        {
            ComPort.DiscardInBuffer();
            ComPort.Write("IOUT1?");
            return GetAndConvertResponce();
        }

        [Flags]
        public enum Status
        {
            ConstV1 = 0x01,
            ConstV2 = 0x02,
            TrackingSeries = 0x04,
            TrackingParrallel = 0x0C,
            Beep = 0x10,
            UnLocked = 0x20,
            OutputOn = 0x40,
        };

        public Status GetStatus()
        {
            ComPort.DiscardInBuffer();
            ComPort.Write("STATUS?");
            byte[] responce= new byte[1];
            while(ComPort.BytesToRead == 0)
            {

            }
            ComPort.Read(responce,0,1);
            return (Status)responce[0];
        }

        public void On()
        {
            ComPort.Write("OUT1");
        }

        public void Off()
        {
            ComPort.Write("OUT0");
        }

        public void Beep(bool BeepEnabled)
        {
            if (BeepEnabled)
                ComPort.Write("BEEP1");
            else
                ComPort.Write("BEEP0");
        }

        public void OVP(bool OVPEnabled)
        {
            if (OVPEnabled)
                ComPort.Write("OVP1");
            else
                ComPort.Write("OVP0");
        }

        public void OCP(bool OCPEnabled)
        {
            if (OCPEnabled)
                ComPort.Write("OCP1");
            else
                ComPort.Write("OCP0");
        }

        public void SaveMemory(int bank)
        {
            if (bank > 0 && bank <= 5)
            {
                var message = "SAV" + bank;
                ComPort.Write(message);
            }
            else
            {
                throw (new ArgumentException("value given is not a valid memory bank number"));
            }
        }

        public void RecallMemory(int bank)
        {
            if (bank > 0 && bank <= 5)
            {
                var message = "RCL" + bank;
                ComPort.Write(message);
            }
            else
            {
                throw (new ArgumentException("value given is not a valid memory bank number"));
            }
        }

        public string GetIDInfo()
        {
            ComPort.DiscardInBuffer();
            ComPort.Write("*IDN?");
            SpinWait.SpinUntil(ComPort.BytesToRead < 18, 2000);
            var result = ComPort.ReadExisting();

            return result;
        }

    }
}
