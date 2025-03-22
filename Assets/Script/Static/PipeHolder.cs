using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;

public static class PipeHolder
{
    private const string PIPE_NAME = @"\\.\pipe\UnityRL";
    private static bool isConnected = false;
    private static TcpListener listener;
    private static TcpClient client;
    private static Queue<string> sendingQueue;
    private static (string,float,string)[] answerBuffer;
    private static object lockObj = new object();

    public static void Initialize()
    {
        try
        {
            if (!isConnected && listener == null) // Check if listener is null before creating a new one
            {
                IPAddress localAdd = IPAddress.Parse("127.0.0.1");
                listener = new TcpListener(localAdd, 25005);
                listener.Start();

                client = listener.AcceptTcpClient();
                Debug.Log("Client connected.");
                isConnected = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Pipe Connection Error: {e.Message}");
        }
    }
    public static void StartAIAgent()
    {
        if (!isConnected)
        {
            Initialize();
        }
        NetworkStream nwStream = client.GetStream();
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Init");
        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        sendingQueue = new();
        answerBuffer = new (string, float, string)[] { ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, ""), ("", -1, "") };
        Task.Run(() => Messenger());
    }
    
    public static async Task<string> SendObservationAsync(string observation)
    {
        return await Task.Run(() => SendObservation(observation));
    }
    public static string SendObservation(string observation)
    {
        sendingQueue.Enqueue(observation);
        string[] command = observation.Split(',');

        int timeout = 500; // Timeout in 500ms
        while (timeout > 0)
        {
            int index = Int32.Parse(command[1]);
            if (!string.IsNullOrEmpty(answerBuffer[index].Item1))
            {
                string action = answerBuffer[index].Item1;
                answerBuffer[index] = ("", -1, "");
                return action;
            }
            Thread.Sleep(10);
            timeout -= 10;
        }
        return $"Action,{command[1]},0,0,0,0,0,0,0"; // Return a default action if timeout occurs
    }
    public static void SendCommmand(string message)
    {
        sendingQueue.Enqueue(message);
    }
    #region Deprecated
    //public static async Task<string> SendObservationAsync(string observation)
    //{
    //    return await Task.Run(() => SendObservation(observation));
    //}

    //public static string SendObservation(string observation)
    //{
    //    if (!isConnected)
    //    {
    //        Initialize();
    //    }
    //    lock (lockObj)
    //    {
    //        try
    //        {
    //            Debug.Log("In Locker");
    //            NetworkStream nwStream = client.GetStream();

    //            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(observation); 
    //            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);

    //            DateTime startTime = DateTime.Now;
    //            Debug.Log("Sent?");

    //            while ((DateTime.Now - startTime).TotalMilliseconds < 5000)
    //            {
    //                nwStream = client.GetStream();
    //                byte[] buffer = new byte[client.ReceiveBufferSize];
    //                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); 
    //                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead); 

    //                if(!string.IsNullOrEmpty(data))
    //                {
    //                    string[] command = data.Split(',');
    //                    if (command[0] == "Action")
    //                        return data;
    //                    else Debug.LogWarning("Not me message");
    //                }

    //                Thread.Sleep(10); 
    //            }
    //            return "Error";
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"Pipe Error: {e.Message}");
    //            isConnected = false;
    //            return "Error";
    //        }
    //    }

    //}

    //public static async Task<string> SendRewardAsync(string observation)
    //{
    //    return await Task.Run(() => SendReward(observation));
    //}

    //public static string SendReward(string reward)
    //{
    //    if (!isConnected)
    //    {
    //        Initialize();
    //    }
    //    lock (lockObj)
    //    {
    //        try
    //        {
    //            NetworkStream nwStream = client.GetStream();

    //            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(reward);
    //            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);

    //            return "Complete";
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"Pipe Error: {e.Message}");
    //            isConnected = false;
    //            return "ERROR";
    //        }

    //    }

    //}
    #endregion



    private static void Messenger()
    {
        while (true)
        {
            if (!isConnected || client == null)
            {
                Initialize();
                if (!isConnected || client == null)
                {
                    Thread.Sleep(100);
                    continue;
                }
            }
            try
            {
                NetworkStream nwStream = client.GetStream();
                if (nwStream == null)
                {
                    throw new Exception("NetworkStream is null");
                }
                if (nwStream.DataAvailable)
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] dataArr = data.Split('&');
                    foreach(string d in dataArr)
                    {
                        if (string.IsNullOrEmpty(d)) continue;
                        string[] command = d.Split(',');

                        if (command[0] == "Action")
                        {
                            int index = int.Parse(command[1]);
                            if (index >= 0 && index < answerBuffer.Length)
                                answerBuffer[index] = (d, 2, "");
                        }
                        else
                        {
                            Debug.LogWarning("Not me message");
                        }
                    }
                    
                }
                else
                {
                    if (sendingQueue.Count > 0)
                    {
                        nwStream = client.GetStream();
                        string observation = sendingQueue.Dequeue();
                        if (observation == null) continue;
                        observation += "&";
                        string[] command = observation.Split(',');
                        byte[] myWriteBuffer = Encoding.ASCII.GetBytes(observation);
                        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);

                        if (command[0] == "Observe")
                        {
                            int index = int.Parse(command[1]);
                            if (index >= 0 && index < answerBuffer.Length)
                                answerBuffer[index] = ("", 2, observation);
                        }
                    }
                }

                for (int i = 0; i < 6; i++)
                {
                    if (!string.IsNullOrEmpty(answerBuffer[i].Item3))
                    {
                        answerBuffer[i].Item2 -= 0.01f;
                        if (answerBuffer[i].Item2 < 0)
                        {
                            answerBuffer[i].Item1 = "Action,0,0,0,0,0,0,0,0,0";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Pipe Error: {e.Message}");
                isConnected = false;

                Close();
                Thread.Sleep(500); // Wait a bit before retrying
                End();
                StartAIAgent();
                return;
            }

            Thread.Sleep(10);
        }
    }

    public static void End()
    {
        if (!isConnected)
        {
            Initialize();
        }
        NetworkStream nwStream = client.GetStream();
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes("End");
        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
    }


    public static void Close()
    {
        if (isConnected)
        {
            try
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
                if (listener != null)
                {
                    listener.Stop();
                    listener = null;
                }

                isConnected = false;
                Debug.Log("Pipe connection closed.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error closing connection: {e.Message}");
            }
        }
    }
    //To do: If fail, exit and run again
}
