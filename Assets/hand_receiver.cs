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
    public int send_port = 22222;
    private UdpClient udpClient;
    private Thread receiveThread;
    private bool running = true;
    Vector3 euler = new Vector3();
    Vector3 cor_euler = new Vector3();
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
        transform.eulerAngles = euler;
    }

    private void ReceiveData()
    {
        while (running)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                string[] split = text.Split(',');
                if (split.Length == 9)
                {
                    euler = new Quaternion(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])).eulerAngles;
                    cor_euler = euler;
                    if (cor_euler.x < 360 && cor_euler.x > 180)
                    {
                        cor_euler.x = cor_euler.x - 360;
                    }
                    if (cor_euler.y < 360 && cor_euler.y > 180)
                    {
                        cor_euler.y = cor_euler.y - 360;
                    }
                    if (cor_euler.z < 360 && cor_euler.z > 180)
                    {
                        cor_euler.z = cor_euler.z - 360;
                    }
                    int power_x = (int)cor_euler.x * 256 / 90;
                    int power_y = (int)cor_euler.z * 256 / 90;
                    int power_z = (int)cor_euler.y * 256 / 90;
                    string message = $"{power_x},{power_y},{power_z},{split[4]},{split[5]},{split[6]},{split[7]},{split[8]}";
                    Debug.Log(message);
                    byte[] send_data = Encoding.UTF8.GetBytes(message);
                    try
                    {
                        udpClient.Send(send_data, send_data.Length, send_IP, port);
                    }
                    catch { Debug.Log("senderror"); }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // スレッド破棄時の例外なので何もしない
            }
            catch (System.Net.Sockets.SocketException)
            {
                // udpClient.Close() 時に発生するので、停止中なら無視する
                if (running) Debug.LogWarning("Socket Error occurred.");
            }
            catch (System.Exception e)
            {
                // それ以外の本当のエラーだけログに出す
                if (running) Debug.LogWarning(e);
            }
        }
    }

    void OnDisable()
    {
        running = false;
        if (udpClient != null) udpClient.Close();
        if (receiveThread != null)
        {
            if (!receiveThread.Join(100)) receiveThread.Abort();
        }
    }

}