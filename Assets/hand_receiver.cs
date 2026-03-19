using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class hand_receiver : MonoBehaviour
{
    public int port = 20000;
    private UdpClient udpClient;
    private Thread  receiveThread;
    [SerializeField] private string send_IP;

    // Start is called before the first frame update
    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ReceiveData()
    {
        while(true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                string[] split = text.Split(',');
                if (split.Length == 9)
                {
                    Vector3 euler = new Quaternion(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])).eulerAngles;
                    int power_x = (int)euler.x * 256 / 90;
                    int power_y = (int)euler.z * 256 / 90;
                    int power_z = (int)euler.y * 256 / 90;
                    string message = $"{power_x},{power_y},{power_z},{split[4]},{split[5]},{split[6]},{split[7]},{split[8]}";
                    Debug.Log(message);
                    byte[] send_data = Encoding.UTF8.GetBytes(message);
                    try
                    {
                        udpClient.Send(send_data, send_data.Length, send_IP, port);
                    }
                    catch { /* ëóêMÉGÉâÅ[ñ≥éã */ }
                }
            }
            catch (System.Exception e) { Debug.LogWarning(e); }
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        udpClient.Close();
    }

}
