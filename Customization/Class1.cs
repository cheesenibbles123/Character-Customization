using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using BWModLoader;
using System.IO;
using Harmony;
using System.Runtime.InteropServices;

namespace CharacterOutfitChanger
{
    internal static class Log
    {
        static readonly public ModLogger logger = new ModLogger("[CharacterOutfitChanger]", ModLoader.LogPath + "\\CharacterOutfitChanger.txt");
    }

    [Mod]
    public class Mainmod : MonoBehaviour
    {
        Texture2D watermarkTex;

        static string texturesFilePath = "/Managed/Mods/Assets/Archie/Textures/";
        static int logLevel = 1;

        void Start()
        {
            logHigh("Patching...");
            HarmonyInstance harmony = HarmonyInstance.Create("com.github.archie");
            harmony.PatchAll();
            logHigh("Patched!");
            if (!createReadme())
            {
                Log.logger.Log("ReadMe creation failed!");
            }
            logHigh("Starting Coroutines...");
            createDirectories();
        }

        void OnGUI()
        {
            if (watermarkTex != null)
            {
                GUI.DrawTexture(new Rect(10, 10, 64, 52), watermarkTex, ScaleMode.ScaleToFit);
            }
        }

        bool createReadme()
        {
            try
            {
                string readMeFilePath = "/Managed/Mods/Assets/Archie/";
                if (!System.IO.File.Exists(Application.dataPath + readMeFilePath + "Readme.txt"))
                {
                    System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(Application.dataPath + readMeFilePath + "Readme.txt");
                    streamWriter.WriteLine("The mod currently allows you to alter:");
                    streamWriter.WriteLine(" - Outfit alb,met,nrm");
                    streamWriter.WriteLine(" - Hat alb,met,nrm");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("The following are directories that are used:");
                    streamWriter.WriteLine("Textures = /Managed/Mods/Assets/Archie/Textures/NAMEOFINGAMEIMG.png");
                    streamWriter.WriteLine(" - Only supported format is '.png'");
                    streamWriter.Close();
                    Log.logger.Log("ReadMe Created!");
                    return true;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.logger.Log(e.Message);
                return false;
            }
        }

        //Adding to allow customization on the user end later on
        void createBadgeFile()
        {
            string badgeFilePath = Application.dataPath + "/Managed/Mods/Assets/Archie/";
            if (!System.IO.File.Exists(badgeFilePath + "badgeList.txt"))
            {
                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(badgeFilePath + "badgeList.txt");
                streamWriter.WriteLine("USERNAME=BADGENAME");
                streamWriter.WriteLine("USERNAME2=BADGENAME2");
                streamWriter.Close();
                Log.logger.Log("ReadMe Created!");
            }
        }

        bool loadBadgeFile()
        {
            string badgeFilePath = Application.dataPath + "/Managed/Mods/Assets/Archie/badgeList.txt";
            if (File.Exists(badgeFilePath))
            {
                string[] badgeFile = File.ReadAllLines(badgeFilePath);
                Log.logger.Log($"Read badge file: {badgeFile.Length} lines found");

                for (int i = 0; i < badgeFile.Length; i++)
                {
                    try
                    {
                        string[] splitArr = badgeFile[i].Split(new char[] { '=' });
                        //PlayerID.Add(splitArr[i]);
                        //badgeName.Add(splitArr[i]);
                    }
                    catch (Exception e)
                    {
                        Log.logger.Log("Error loading badge file into program:");
                        Log.logger.Log(e.Message);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        //////////////////////////////////////////////////////

        private IEnumerator waterMark()
        {
            if (!File.Exists(Application.dataPath + texturesFilePath + "pfp.png"))
            {
                logMed("pfp not found");
                WWW www = new WWW("http://www.archiesbots.com/BlackwakeStuff/pfp.png");
                yield return www;

                try
                {
                    byte[] bytes = www.texture.EncodeToPNG();
                    logHigh("Encoded bytes");
                    File.WriteAllBytes(Application.dataPath + texturesFilePath + "pfp.png", bytes);
                    logHigh("Written files");
                }
                catch (Exception e)
                {
                    logMed("Error downloading watermark:");
                    logMed(e.Message);
                }

            }
            else
            {
                logMed("pfp found");
            }

            watermarkTex = loadTexture("pfp", 258, 208);
        }

        void createDirectories()
        {
            if (!File.Exists(Application.dataPath + texturesFilePath))
            {
                Directory.CreateDirectory(Application.dataPath + texturesFilePath);
            }

            StartCoroutine(waterMark());
        }

        static void logLow(string message)
        {
            if (logLevel > 0)
            {
                Log.logger.Log(message);
            }
        }

        static void logMed(string message)
        {
            if (logLevel > 1)
            {
                Log.logger.Log(message);
            }
        }

        static void logHigh(string message)
        {
            if (logLevel > 2)
            {
                Log.logger.Log(message);
            }
        }

        static Texture2D loadTexture(string texName, int imgWidth, int imgHeight)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(Application.dataPath + texturesFilePath + texName + ".png");

                Texture2D tex = new Texture2D(imgWidth, imgHeight, TextureFormat.RGB24, false);
                tex.LoadImage(fileData);
                return tex;

            }
            catch (Exception e)
            {
                logLow(string.Format("Error loading texture {0}", texName));
                logLow(e.Message);
                return Texture2D.whiteTexture;
            }
        }

        [HarmonyPatch(typeof(PlayerBuilder), "òêîîòçóêçìï", new Type[] { typeof(OutfitItem), typeof(OutfitItem), typeof(OutfitItem), typeof(OutfitItem), typeof(OutfitItem), typeof(Transform), typeof(int) })]
        static class PlayerBuilderPatch
        {
            static void Postfix(PlayerBuilder __instance, OutfitItem êïòèíèñæêðé, OutfitItem êëëðîñìîçíê, OutfitItem îèòçòñïéèêê, OutfitItem ðçëìèêîïæäè, OutfitItem òìðêäëêääåï, Transform ëççêîèåçåäï, int óïèññðæìëòè)
            {
                logMed("Entered Patch");

                //MAINOUTFIT
                string texName;
                int imgWH = 2048;

                try
                {
                    if (__instance.êêïæëëëòììó.material.HasProperty("_ClothesAlbedoFuzzMask"))
                    {
                        texName = __instance.êêïæëëëòììó.material.GetTexture("_ClothesAlbedoFuzzMask").name;
                        logMed($"ALB found, name: {texName}");

                        if (File.Exists(Application.dataPath + texturesFilePath + texName + ".png"))
                        {
                            Texture2D tex = loadTexture(texName, imgWH, imgWH);
                            logHigh("Loaded texture for Alb");

                            __instance.êêïæëëëòììó.material.SetTexture("_ClothesAlbedoFuzzMask", tex);
                            logHigh("Set texture for Alb");
                        }
                    }

                    if (__instance.êêïæëëëòììó.material.HasProperty("_ClothesMetSmoothOccBlood"))
                    {
                        texName = __instance.êêïæëëëòììó.material.GetTexture("_ClothesMetSmoothOccBlood").name;
                        logMed($"ALB found, name: {texName}");

                        if (File.Exists(Application.dataPath + texturesFilePath + texName + ".png"))
                        {
                            Texture2D tex = loadTexture(texName, imgWH, imgWH);
                            logHigh("Loaded texture for Clothes Met");

                            __instance.êêïæëëëòììó.material.SetTexture("_ClothesMetSmoothOccBlood", tex);
                            logHigh("Set texture for Clothes Met");
                        }
                    }

                    if (__instance.êêïæëëëòììó.material.HasProperty("_ClothesBump"))
                    {
                        texName = __instance.êêïæëëëòììó.material.GetTexture("_ClothesBump").name;
                        logMed($"ALB found, name: {texName}");

                        if (File.Exists(Application.dataPath + texturesFilePath + texName + ".png"))
                        {
                            Texture2D tex = loadTexture(texName, imgWH, imgWH);
                            logHigh("Loaded texture for Clothes Bump");

                            __instance.êêïæëëëòììó.material.SetTexture("_ClothesBump", tex);
                            logHigh("Set texture for Clothes Bump");
                        }
                    }

                }
                catch (Exception e)
                {
                    logLow("Error loading outfit texture");
                    logLow(e.Message);
                }

                //HATS
                try
                {
                    if (__instance.êêïæëëëòììó.material.HasProperty("_HatAOMetallic"))
                    {
                        texName = __instance.êêïæëëëòììó.material.GetTexture("_HatAlbedoSmooth").name;
                        logMed($"ALB found, name: {texName}");

                        if (File.Exists(Application.dataPath + texturesFilePath + texName + ".png"))
                        {
                            Texture2D tex = loadTexture(texName, imgWH, imgWH);
                            logHigh("Loaded texture for Hat Alb");

                            __instance.êêïæëëëòììó.material.SetTexture("_HatAlbedoSmooth", tex);
                            logHigh("Loaded texture for Hat Alb");
                        }
                    }

                    if (__instance.êêïæëëëòììó.material.HasProperty("_HatAOMetallic"))
                    {
                        texName = __instance.êêïæëëëòììó.material.GetTexture("_HatAOMetallic").name;
                        logMed($"ALB found, name: {texName}");

                        if (File.Exists(Application.dataPath + texturesFilePath + texName + ".png"))
                        {
                            Texture2D tex = loadTexture(texName, imgWH, imgWH);
                            logHigh("Loaded texture for Hat Met");

                            __instance.êêïæëëëòììó.material.SetTexture("_HatAOMetallic", tex);
                            logHigh("Loaded texture for Hat Met");
                        }
                    }

                    if (__instance.êêïæëëëòììó.material.HasProperty("_HatBump"))
                    {
                        texName = __instance.êêïæëëëòììó.material.GetTexture("_HatBump").name;
                        logMed($"ALB found, name: {texName}");

                        if (File.Exists(Application.dataPath + texturesFilePath + texName + ".png"))
                        {
                            Texture2D tex = loadTexture(texName, imgWH, imgWH);
                            logHigh("Loaded texture for Hat Bump");

                            __instance.êêïæëëëòììó.material.SetTexture("_HatBump", tex);
                            logHigh("Loaded texture for Hat Bump");
                        }
                    }
                }
                catch (Exception e)
                {
                    logLow("Error loading hat texture");
                    logLow(e.Message);
                }
            }
        }

    }

}