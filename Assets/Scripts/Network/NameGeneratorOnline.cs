using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public static class NameGeneratorOnline
    {
        private const string FantasyNameSite = @"https://www.fantasynamegenerators.com/steampunk-names.php";

        public static IEnumerator GetRandomName(Action<string> onSuccess, Action onError)
        {

            UnityWebRequest webRequest = new UnityWebRequest(FantasyNameSite);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError == true || webRequest.isHttpError == true)
            {
                Debug.LogError($"An error has occured while downloading fantasy name: {webRequest.error}");
                onError?.Invoke();
                yield break;
            }

            string html = webRequest.downloadHandler.text;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//div[@id]");
            HtmlNode node = doc.DocumentNode.SelectSingleNode(@"//div[@id]");
            if (node == null)
            {
                Debug.LogError($"Node with id='result' not found.");
                onError?.Invoke();
                yield break;
            }
        }

    }
}
