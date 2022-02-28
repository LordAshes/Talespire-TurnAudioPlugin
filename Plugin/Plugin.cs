using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    public partial class TurnAudioPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Turn Audio Plug-In";              
        public const string Guid = "org.lordashes.plugins.turnaudio";
        public const string Version = "1.0.0.0";                    

        // Configuration
        private bool subscribed = false;
        private bool globalDistribution = false;

        void Awake()
        {
            UnityEngine.Debug.Log("Turn Audio Plugin: Active ("+this.GetType().AssemblyQualifiedName+").");
            globalDistribution = Config.Bind("Settings", "Global Distribution", false).Value;

            Utility.PostOnMainPage(this.GetType());
        }

        void Update()
        {
            if (Utility.isBoardLoaded())
            {
                if (!subscribed) { InitiativeManager.OnTurnSwitch += TurnHandler; subscribed = true; }
            }
            else
            {
                if (subscribed) { InitiativeManager.OnTurnSwitch -= TurnHandler; subscribed = false; }
            }
        }

        private void TurnHandler(CreatureGuid cid)
        {
            CreatureBoardAsset asset;
            CreaturePresenter.TryGetAsset(cid, out asset);
            if (asset != null)
            {
                Debug.Log("Turn Audio Plugin: It is now " + asset.Creature.Name + "'s Turn");
                if(globalDistribution || LocalClient.CanControlCreature(cid))
                {
                    string source = null;
                    if(FileAccessPlugin.File.Exists("Audio/"+GetCreatureName(asset.Creature.Name)))
                    {
                        source = FileAccessPlugin.File.Find("Audio/" + GetCreatureName(asset.Creature.Name))[0];
                    }
                    else if (FileAccessPlugin.File.Exists("Audio/Default"))
                    {
                        source = FileAccessPlugin.File.Find("Audio/Default")[0];
                    }
                    if(source!=null)
                    {
                        StartCoroutine("PlayAudio", new object[] { source });
                    }
                }
            }
        }

        private IEnumerator PlayAudio(object[] inputs)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip((string)inputs[0], AudioType.UNKNOWN))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("Audio Plugin: Failure To Load "+(string)inputs[0]+"...");
                    Debug.Log(www.error);
                }
                else
                {
                    GameObject speaker = GameObject.Find("InitiativeSpeaker");
                    if (speaker == null) 
                    { 
                        speaker = new GameObject(); 
                        speaker.name = "InitiativeSpeaker";
                        speaker.AddComponent<AudioSource>();
                    }
                    AudioSource player = speaker.GetComponent<AudioSource>();
                    player.clip = DownloadHandlerAudioClip.GetContent(www);
                    player.Play();
                }
            }
        }

        private static string GetCreatureName(string statBlock)
        {
            if(statBlock.Contains("<"))
            {
                statBlock = statBlock.Substring(0, statBlock.IndexOf("<"));
            }
            return statBlock;
        }
    }
}
