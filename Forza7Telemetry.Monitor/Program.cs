﻿using System;
using System.Net;
using System.Net.Sockets;
using Forza7Telemetry.Monitor.Telemetry;
using Newtonsoft.Json;

namespace Forza7Telemetry.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new TelemetryListener(11001, (data) =>
            {
                var message = new Message(data);
                Console.SetCursorPosition(0, 0);

                if (message.IsRaceOn == 1)
                {
                    Console.WriteLine("RPM: {0}          ", message.CurrentEngineRpm);
                    Console.WriteLine("Velocity - X: {0}, Y: {1}, Z: {2}         ", message.VelocityX, message.VelocityY, message.VelocityZ);
                    Console.WriteLine("Acceleration - X: {0}, Y: {1}, Z: {2}      ", message.AccelerationX, message.AccelerationY, message.AccelerationZ);
                }
                else
                {
                    Console.WriteLine("Waiting for race to start");
                }
            });

            listener.StartListener();
        }
    }

    class ForzaListener
    {
        public static void StartListener()
        {
            bool done = false;

            UdpClient listener = new UdpClient(11000);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 11000);

            try
            {
                while (!done)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    var raceStarted = BitConverter.ToInt32(bytes, 0);
                    if (raceStarted == 1)
                    {
                        var rpm = BitConverter.ToSingle(bytes, 16);
                        Console.WriteLine("RPM: {0}", rpm);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
