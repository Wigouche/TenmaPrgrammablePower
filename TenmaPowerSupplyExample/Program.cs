using System;
using System.Collections.Generic;
using TenmaPrgrammablePower;
using System.IO.Ports;
using System.Threading;


namespace TenmaPowerSupplyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("please Select the Comport for the power Supply");
            TenmaSupply supply = new TenmaSupply( SetPortName());

            Console.WriteLine(supply.GetIDInfo());
            Thread.Sleep(500);
            supply.Beep(false);
            Thread.Sleep(500);
            Console.WriteLine(supply.GetStatus());
            Thread.Sleep(500);

            Console.WriteLine("Press Enter To Continue");
            Console.ReadLine();
            Console.WriteLine("Set supply to 52V at 700mA limt and make sure its ON");

            supply.SetV(51M);
            Thread.Sleep(500);
            supply.SetI(.7M);
            Thread.Sleep(500);
            supply.On();

            Console.WriteLine("Press Enter To Continue");
            Console.ReadLine();
            Console.WriteLine("Read power suppy voltage and current set points and measurements");

            Console.WriteLine("Set points {0:#0.00} V, {1:0.000} A", supply.GetVSet(), supply.GetISet());
            Thread.Sleep(500);
            Console.WriteLine("Output measurments {0:#0.00} V, {1:0.000} A", supply.GetVOut(), supply.GetIOut());

            Console.WriteLine("Press Enter To Exit");
            Console.ReadLine();
        }

        public static string SetPortName()
        {

            List<String> AvalablePortNames = new List<String>();
            string portName = null;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                AvalablePortNames.Add(s);
            }
            for (int i = 0; i < AvalablePortNames.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i, AvalablePortNames[i]);
            }

            Console.WriteLine("Enter COM port index value from list(Default: 0. {0}): ", AvalablePortNames[0]);
            while (portName == null)
            {
                string portToSelectInput = Console.ReadLine();

                if (portToSelectInput == "")
                {
                    portName = AvalablePortNames[0];
                }
                else
                {

                    if (int.TryParse(portToSelectInput, out int portToSelect))
                    {
                        if (portToSelect < AvalablePortNames.Count && portToSelect >= 0)
                        {
                            portName = AvalablePortNames[portToSelect];
                        }
                        else
                        {
                            Console.WriteLine("{0} is not a valid index from the list", portToSelect);
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0} is not a valid index from the list", portToSelectInput);
                    }
                }
            }
            return portName;
        }
    }
}
