using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;

public class IpLoader : MonoBehaviour
{
    public GameObject ipText;

    private void Update()
    {
        ipText.GetComponent<TextMeshProUGUI>().text = "local ip:\n" + GetLocalIPv4();
    }
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }
}
