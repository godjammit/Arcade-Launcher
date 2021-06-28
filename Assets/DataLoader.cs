using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class DataLoader
{
    private List<GameAsset> gameAssetsList;

    public DataLoader()
    {
        GetAssets();
    }

    private void GetAssets()
    {
        gameAssetsList = new List<GameAsset>();

        string[] allDirs = Directory.GetDirectories(Environment.CurrentDirectory);
        foreach (string s in allDirs)
        {
            string[] info = Directory.GetFiles(s, "info.txt");
            if (info.Any())
            {
                Debug.Log("got info");
                GameAsset asset = new GameAsset();

                // get Game
                string text = File.ReadAllText(info[0]);
                Game game = JsonConvert.DeserializeObject<Game>(text);
                asset.GameData = game;

                // get Texture
                string[] textures = Directory.GetFiles(s, "card.png");
                if (textures.Any())
                {
                    WWW www = new WWW("file://" + textures[0]);
                    asset.Card = www.texture;
                    Debug.Log("got texture");
                }

                // get executable path
                asset.ExecutablePath = s + "/" + asset.GameData.Executable;
                Debug.Log("got executable " + asset.ExecutablePath);
                
                gameAssetsList.Add(asset);
            }
        }
    }

    public List<GameAsset> GetGameAssetsList()
    {
        return gameAssetsList;
    }
}