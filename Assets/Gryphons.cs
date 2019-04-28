using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using KModkit;


public class Gryphons : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    public Texture[] birdImages;
    public Texture[] catImages;
    public Texture[] accessoryImages;

    public KMSelectable birdUp;
    public KMSelectable birdDown;

    public KMSelectable catUp;
    public KMSelectable catDown;

    public KMSelectable accessoryLeft;
    public KMSelectable accessoryRight;

    public KMSelectable submit;

    //public MeshRenderer display;
        
    //public KMSelectable button;

    public MeshRenderer birdPart;
    public MeshRenderer catPart;
    public MeshRenderer accessoryPart;

    public MeshRenderer typeBox;
    public MeshRenderer accessoryBox;
    public MeshRenderer IDBox;

    //public KMRuleSeedable RuleSeedable;
    string[] gryphNames = new string[100]
    { "Gabe", "Gabriel", "Gad", "Gael", "Gage", "Gaia", "Galena", "Galina", "Gallo", "Gallagher", "Ganymede", "Ganzorig", "Garen", "Gareth", "Garland", "Garnett", "Garret",
        "Garrick", "Gary", "Gaspar", "Gaston", "Gauthier", "Gavin", "Gaz", "Geena", "Geff", "Geffrey", "Gela", "Geltrude", "Gene", "Geneva", "Genevieve", "Geno", "Gentius",
        "Geoff", "George", "Georgio", "Georgius", "Gerald", "Geraldo", "Gerda", "Gerel", "Gergana", "Gerhardt", "Gerhart", "Gerry", "Gertrude", "Gervais", "Gervaise", "Ghada",
        "Ghadir", "Ghassan", "Ghjulia", "Gia", "Giada", "Giampaolo", "Giampiero", "Giancarlo", "Giana", "Gianna", "Gideon", "Gidon", "Gilbert", "Gilberta", "Gino", "Giorgio",
        "Giovanni", "Giove", "Girish", "Girisha", "Gisela", "Giselle", "Gittel", "Gizella", "Gjorgji", "Gladys", "Glauco", "Glaukos", "Glen", "Glenn", "Godfrey", "Godfried",
        "Gojko", "Gol", "Golda", "Gona", "Gonzalo", "Gordie", "Gordy", "Goretti", "Gosia", "Gosse", "Gotzon", "Gotzone", "Gowri", "Gozzo", "Grace", "Gracia", "Griffith", "Gwynnyth"
    };

    string[] accessories = new string[6]
    {
        "Watch", "Visor", "Shoes",
        "Scarf", "Headphones", "Shades",
    };    
	
	string[] birds = new string[6]
    {
        "Eagle", "Falcon", "Peacock",
        "Cardinal", "Blue Jay", "Crow",
    };    


	string[] cats = new string[6]
    {
        "Tiger", "Lion", "Cheetah",
        "Panther", "Snow Leopard", "Housecat",
    };

    string[] alphabet = new string[26]
    { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
      "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};

    /*
    int[] values = new int[26] 
    { 1, 5, 5, 5, 1, 5, 5, 6, 1, 3, 6, 4, 6,
      4, 1, 6, 3, 4, 4, 4, 1, 6, 2, 3, 2, 3 };
  
         */

    int[,] extraValues = new int[,]
    {
        {9, 2, -2, 4, 2, 4 },
        {5, 4, 8, 5, 4, 5 },
        {4, 5, 2, 4, 7, 2 },
        {2, 4, -5, 2, 5, 10 },
        {5, 4, 2, 5, 11, 5 },
        {5, 2, 4, 12, 2, -4 }
    };



    bool pressedAllowed = false;

    // TWITCH PLAYS SUPPORT
    //int tpStages; This one is not needed for this module
    // TWITCH PLAYS SUPPORT

    int age;
    int pickedName;

    int correctBird;
    int correctCat;
    int correctAccessory;

    int currentBird;
    int currentCat;
    int currentAccessory;

    int tableColumn = 0;
    int tableRow = 0;

    bool isSolved = false;
    bool tpActive = false;
    
    void Start()
    {
        _moduleId = _moduleIdCounter++;
        //colorblindModeEnabled = colorblindMode.ColorblindModeActive;
        Init();
        pressedAllowed = true;
    }

    void Init()
    {
        delegationZone();
        currentBird = UnityEngine.Random.Range(0, 6);
        birdPart.material.mainTexture = birdImages[currentBird];
        currentCat = UnityEngine.Random.Range(0, 6);
        catPart.material.mainTexture = catImages[currentCat];
        currentAccessory = UnityEngine.Random.Range(0, 6);
        accessoryPart.material.mainTexture = accessoryImages[currentAccessory];
        pickedName = UnityEngine.Random.Range(0, gryphNames.Count());
        age = UnityEngine.Random.Range(23, 35);
        IDBox.GetComponentInChildren<TextMesh>().text = gryphNames[pickedName] + " (" + age + ")";
        typeBox.GetComponentInChildren<TextMesh>().text = birds[currentBird] + "/" + cats[currentCat];
        accessoryBox.GetComponentInChildren<TextMesh>().text = accessories[currentAccessory];
        var spaceNum = age - 1;
        var getCharA = "";
        var getCharB = "";
        var advanceValueA = 0;
        var advanceValueB = 0;

        tableColumn = spaceNum % 6;
        tableRow = spaceNum / 6;
        

        if (gryphNames[pickedName].Length % 4 == 0)
        {
            getCharA = Bomb.GetSerialNumber().Substring(0, 1);
            Debug.LogFormat("[Gryphons #{0}] The name, {1}, has a length divisible by four, so using the first SN character, which is {2}.", _moduleId, gryphNames[pickedName], getCharA);
        }
        else if (gryphNames[pickedName].Where(x => "i".Contains(x)).Count() == 0 && gryphNames[pickedName].Where(x => "e".Contains(x)).Count() == 0)
        {
            getCharA = Bomb.GetSerialNumber().Substring(0, 1);
            Debug.LogFormat("[Gryphons #{0}] The name, {1}, does not contain an 'I' or an 'E', so using the first SN character, which is {2}.", _moduleId, gryphNames[pickedName], getCharA);
        }
        else
        {
            getCharA = Bomb.GetSerialNumber().Substring(4, 1);
            Debug.LogFormat("[Gryphons #{0}] The name, {1}, has a length that is not divisible by four, and contains either an 'I' or an 'E', so using the fifth SN character, which is {2}."
                , _moduleId, gryphNames[pickedName], getCharA);
        }
        if (getCharA.TryParseInt() >= 0 && getCharA.TryParseInt() <= 9)
        {
            advanceValueA = Int16.Parse(getCharA);
            Debug.LogFormat("[Gryphons #{0}] This is a digit.", _moduleId);
        }
        else
        {
            for (int cn = 0; cn < 26; cn++)
            {
                if (alphabet[cn] == getCharA)
                {
                    advanceValueA = cn + 1;
                    Debug.LogFormat("[Gryphons #{0}] This is a letter, position {1} in the alphabet.", _moduleId, advanceValueA);
                    cn = 26;
                }
            }
        }
        spaceNum = spaceNum + advanceValueA;
        tableColumn = (spaceNum) % 6;
        tableRow = (spaceNum / 6) % 6;
        Debug.LogFormat("[Gryphons #{0}] Starting in cell number {7} ({1}/{2}) and advancing {3} cell{6} in reading order. This brings us to {4}/{5}", 
            _moduleId, birds[(age - 1) % 6], cats[(age - 1) / 6], advanceValueA, birds[tableColumn], cats[tableRow], advanceValueA == 1 ? "" : "s", age);

        if (gryphNames[pickedName].Length < 6)
        {
            getCharB = Bomb.GetSerialNumber().Substring(1, 1);
            Debug.LogFormat("[Gryphons #{0}] The name has a length that's less than six letters, so using the second SN character, which is {2}.", _moduleId, gryphNames[pickedName], getCharB);
        }
        else if (gryphNames[pickedName].Where(x => "z".Contains(x)).Count() != 0 || gryphNames[pickedName].Where(x => "u".Contains(x)).Count() != 0)
        {
            getCharB = Bomb.GetSerialNumber().Substring(1, 1);
            Debug.LogFormat("[Gryphons #{0}] The name contains a 'Z' or a 'U', so using the second SN character, which is {2}.", _moduleId, gryphNames[pickedName], getCharB);
        }
        else
        {
            getCharB = Bomb.GetSerialNumber().Substring(3, 1);
            Debug.LogFormat("[Gryphons #{0}] The name has a length that's at least six letters, and does not contain a 'Z' or a 'U', so using the fourth SN character, which is {2}.", 
                _moduleId, gryphNames[pickedName], getCharB);
        }
        if (getCharB.TryParseInt() >= 0 && getCharB.TryParseInt() <= 9)
        {
            advanceValueB = Int16.Parse(getCharB);
            Debug.LogFormat("[Gryphons #{0}] This is a digit.", _moduleId);
        }
        else
        {
            for (int cn = 0; cn < 26; cn++)
            {
                if (alphabet[cn] == getCharB)
                {
                    advanceValueB = cn + 1;
                    Debug.LogFormat("[Gryphons #{0}] This is a letter, position {1} in the alphabet.", _moduleId, advanceValueB);
                    cn = 26;
                }
            }
        }
        var nextStep = "Resuming from " + birds[tableColumn] + "/" + cats[tableRow];

        tableRow = (tableRow + advanceValueB) % 6;
        Debug.LogFormat("[Gryphons #{0}] {1} and going down {2} row{5}. This brings us to our final gryphon type: {3}/{4}.", 
            _moduleId, nextStep, advanceValueB, birds[tableColumn], cats[tableRow], advanceValueB == 1 ? "" : "s");

        var accessoryTotal = extraValues[tableRow, tableColumn] + age;
        var snThree = 0;
        var snSix = 0;
        if (Bomb.GetSerialNumber().Substring(2, 1).TryParseInt() >= 0 && Bomb.GetSerialNumber().Substring(2, 1).TryParseInt() <= 9)
        {
            snThree = Int16.Parse(Bomb.GetSerialNumber().Substring(2, 1));
        }
        else
        {
            for (int cn = 0; cn < 26; cn++)
            {
                if (alphabet[cn] == Bomb.GetSerialNumber().Substring(2, 1))
                {
                    snThree = cn + 1;
                    cn = 26;
                }
            }
        }
        if (Bomb.GetSerialNumber().Substring(5, 1).TryParseInt() >= 0 && Bomb.GetSerialNumber().Substring(5, 1).TryParseInt() <= 9)
        {
            snSix = Int16.Parse(Bomb.GetSerialNumber().Substring(5, 1));
        }
        else
        {
            for (int cn = 0; cn < 26; cn++)
            {
                if (alphabet[cn] == Bomb.GetSerialNumber().Substring(5, 1))
                {
                    snSix = cn + 1;
                    cn = 26;
                }
            }
        }
        accessoryTotal = accessoryTotal + snThree + snSix;
        Debug.LogFormat("[Gryphons #{0}] The number in this cell is {1}. Add the age ({2}), and the third and sixth SN digits, {3} and {4}, gives us a total of {5}. That modulo 6 is {6}, which gives us the accessory {7}.", 
            _moduleId, extraValues[tableRow, tableColumn], age, Bomb.GetSerialNumber().Substring(2, 1), Bomb.GetSerialNumber().Substring(5, 1), accessoryTotal, accessoryTotal % 6, accessories[accessoryTotal % 6]);
        correctAccessory = accessoryTotal % 6;
        correctBird = tableColumn;
        correctCat = tableRow;
        pressedAllowed = true;
    }


    void OnHold()
    {
		
    }

    void OnRelease()
    {
    }
/*
    private void FixedUpdate()
    {
        if (isSolved)
        {
		
        }
    }
*/
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} (submit/s) (Bird type) (Cat type) (Accessory) to submit an answer. The types 'blue jay' and 'snow leopard' can be combined into one word, 'housecat' can be two.";
    private readonly bool TwitchShouldCancelCommand = false;
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        tpActive = true;
        var piecesRaw = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var pieces = new string[] { "bad", "bad", "bad", "bad" };
        

        string theError;
        theError = "";
        yield return null;
        if (piecesRaw.Count() == 0)
        {
            theError = "sendtochaterror No arguments! You need to use submit/s, then a bird type, a cat type, and an accessory (submit bird cat accessory)";
            yield return theError;
        }
        else if (piecesRaw.Count() < 4)
        {
            theError = "sendtochaterror Not enough arguments! You need to use submit/s, then a bird type, a cat type, and an accessory (submit bird cat accessory)";
            yield return theError;
        }
        else
        {
            pieces[0] = piecesRaw[0];
            if ((piecesRaw[1] == "blue" && piecesRaw[2] == "jay") || piecesRaw[1] == "bluejay")
            {
                pieces[1] = "bj";
            }
            else
            {
                pieces[1] = piecesRaw[1];
            }
            if ((piecesRaw[2] == "house" && piecesRaw[3] == "cat") || (piecesRaw[3] == "house" && piecesRaw[4] == "cat") || piecesRaw[2] == "housecat" || piecesRaw[3] == "housecat")
            {
                pieces[2] = "hc";
            }
            if ((piecesRaw[2] == "snow" && piecesRaw[3] == "leopard") || (piecesRaw[3] == "snow" && piecesRaw[4] == "leopard") || piecesRaw[2] == "snowleopard" || piecesRaw[3] == "snowleopard")
            {
                pieces[2] = "sl";
            }
            else
            {
                if (piecesRaw[1] == "blue")
                {

                    pieces[2] = piecesRaw[3];
                }
                else
                {

                    pieces[2] = piecesRaw[2];
                }
            }
            pieces[3] = piecesRaw[piecesRaw.Count() - 1];
            Debug.LogFormat("You entered >>> " + pieces[0] + " " + pieces[1] + " " + pieces[2] + " " + pieces[3] + " <<< which I hope makes sense");
            if (pieces.Count() < 4)
            {
                theError = "sendtochaterror Not enough arguments! You need to use submit/s, then a bird type, a cat type, and an accessory (submit bird cat accessory)";
                yield return theError;
            }
            else if (pieces[0] != "submit" && pieces[0] != "s")
            {
                Debug.Log(piecesRaw[0] + " and " + pieces[0]);
                theError = "sendtochaterror Invalid arguments! You need to use submit/s to submit.";
                yield return theError;
            }
            else if (pieces[1] != "eagle" && pieces[1] != "falcon" && pieces[1] != "peacock" && pieces[1] != "cardinal" && pieces[1] != "bj" && pieces[1] != "crow")
            {
                theError = "sendtochaterror Invalid bird type! Valid types are eagle, falcon, peacock, cardinal, blue jay/bluejay, crow.";
                yield return theError;
            }
            else if (pieces[2] != "tiger" && pieces[2] != "lion" && pieces[2] != "cheetah" && pieces[2] != "panther" && pieces[2] != "sl" && pieces[2] != "hc")
            {
                theError = "sendtochaterror Invalid cat type! Valid types are tiger, lion, cheetah, panther, snow leopard/snowleopard, housecat/house cat.";
                yield return theError;
            }
            else if (pieces[3] != "watch" && pieces[3] != "visor" && pieces[3] != "shoes" && pieces[3] != "scarf" && pieces[3] != "headphones" && pieces[3] != "shades")
            {
                theError = "sendtochaterror Invalid accessory type! Valid types are watch, visor, shoes, scarf, headphones, shades.";
                yield return theError;
            }
            else
            {
                if (pieces[1] == "bj")
                {
                    while (birds[currentBird] != "Blue Jay")
                    {
                        yield return new WaitForSeconds(.1f);
                        yield return null;
                        birdUp.OnInteract();
                    }
                }
                else
                {
                    while (birds[currentBird].ToLowerInvariant() != pieces[1])
                    {
                        yield return new WaitForSeconds(.1f);
                        yield return null;
                        birdUp.OnInteract();
                    }
                }

                if (pieces[2] == "sl")
                {
                    while (cats[currentCat] != "Snow Leopard")
                    {
                        yield return new WaitForSeconds(.1f);
                        yield return null;
                        catUp.OnInteract();
                    }
                }
                else if (pieces[2] == "hc")
                {
                    while (cats[currentCat] != "Housecat")
                    {
                        yield return new WaitForSeconds(.1f);
                        yield return null;
                        catUp.OnInteract();
                    }
                }
                else
                {
                    while (cats[currentCat].ToLowerInvariant() != pieces[2])
                    {
                        yield return new WaitForSeconds(.1f);
                        yield return null;
                        catUp.OnInteract();
                    }
                }
                while (accessories[currentAccessory].ToLowerInvariant() != pieces[3])
                {
                    yield return new WaitForSeconds(.1f);
                    yield return null;
                    accessoryRight.OnInteract();
                }
                yield return new WaitForSeconds(.1f);
                yield return null;
                submit.OnInteract();
            }
        }

		/*
        else if (pieces.Count() >= 1 && pieces[0] == "colorblind")
        {
            colorblindModeEnabled = true;
            colorblindLabel.GetComponentInChildren<TextMesh>().text = colorNames[colorNumber];
            yield return null;
        }
        else if (pieces.Count() == 1 && (pieces[0] == "tap" || pieces[0] == "t" || pieces[0] == "hold" || pieces[0] == "h" || pieces[0] == "release" || pieces[0] == "r"))
        {
            theError = "sendtochaterror Not enough arguments! You need to specify a digit the number of seconds remaining ends in, using !{0} tap/t/hold/h/release/r (1-9).";
            yield return theError;

        }
        else if (pieces.Count() == 1 && !(pieces[0] == "tap" || pieces[0] == "t" || pieces[0] == "hold" || pieces[0] == "h" || pieces[0] == "release" || pieces[0] == "r"))
        {
            theError = "sendtochaterror Invalid argument: " + pieces[0] + "! You need to specify a digit the number of seconds remaining ends in, using !{0} tap/t/hold/h/release/r (1-9).";
            yield return theError;

        }
        else if (pieces.Count() > 1 && !(pieces[0] == "tap" || pieces[0] == "t" || pieces[0] == "hold" || pieces[0] == "h" || pieces[0] == "release" || pieces[0] == "r"))
        {
            theError = "sendtochaterror Invalid argument: " + pieces[0] + "! You need to specify a digit the number of seconds remaining ends in, using !{0} tap/t/hold/h/release/r (1-9).";
            yield return theError;

        }
        else if (pieces.Count() > 1 && !(pieces[1] == "1" || pieces[1] == "2" || pieces[1] == "3" ||
                                        pieces[1] == "6" || pieces[1] == "5" || pieces[1] == "6" ||
                                        pieces[1] == "7" || pieces[1] == "8" || pieces[1] == "9"))
        {
            theError = "sendtochaterror Invalid argument: " + pieces[1] + " is not a digit from 1 to 9! You need to specify a digit the number of seconds remaining ends in, using !{0} tap/t/hold/h/release/r (1-9).";
            yield return theError;
        }
        else if (pieces[0] == "tap" || pieces[0] == "t")
        {
            yield return null;
            holdWait = Int16.Parse(pieces[1]);
            releaseWait = Int16.Parse(pieces[1]);
            if (actionNeeded != 0 || holdWait != neededNumber)
            {
                yield return "strike";
            }
            else
            {
                yield return "solve";
            }
        }
        else if (pieces[0] == "hold" || pieces[0] == "h")
        {
            yield return null;
            holdWait = Int16.Parse(pieces[1]);
            releaseWait = 0;
            if (actionNeeded != 1 || holdWait != neededNumber)
            {
                yield return "strike";
            }
            else
            {
                yield return "solve";
            }
            //           yield return theError;
        }
        else if (pieces[0] == "release" || pieces[0] == "r")
        {
            yield return null;
            holdWait = 0;
            releaseWait = Int16.Parse(pieces[1]);
            if (actionNeeded != 2 || releaseWait != neededNumber)
            {
                yield return "strike";
            }
            else
            {
                yield return "solve";
            }
            //            yield return theError;
        }
		*/
     }

    void CatGoUp()
    {
        currentCat++;
        currentCat = currentCat % 6;
        catPart.material.mainTexture = catImages[currentCat];
    }


    void CatGoDown()
    {
        currentCat = currentCat + 5;
        currentCat = currentCat % 6;
        catPart.material.mainTexture = catImages[currentCat];
    }


    void BirdGoUp()
    {
        currentBird++;
        currentBird = currentBird % 6;
        birdPart.material.mainTexture = birdImages[currentBird];
    }


    void BirdGoDown()
    {
        currentBird = currentBird + 5;
        currentBird = currentBird % 6;
        birdPart.material.mainTexture = birdImages[currentBird];
    }


    void AccessoryGoLeft()
    {
        currentAccessory++;
        currentAccessory = currentAccessory % 6;
        accessoryPart.material.mainTexture = accessoryImages[currentAccessory];
        accessoryBox.GetComponentInChildren<TextMesh>().text = accessories[currentAccessory];
    }


    void AccessoryGoRight()
    {
        currentAccessory = currentAccessory + 5;
        currentAccessory = currentAccessory % 6;
        accessoryPart.material.mainTexture = accessoryImages[currentAccessory];
        accessoryBox.GetComponentInChildren<TextMesh>().text = accessories[currentAccessory];
    }


    void doSubmit()
    {
        if (pressedAllowed)
        {
            if (currentAccessory != correctAccessory)
            {
                Debug.LogFormat("[Gryphons #{0}] {3} is wearing the {1} when we expected the {2}, this is incorrect. Strike given.", 
                    _moduleId, accessories[currentAccessory], accessories[correctAccessory], gryphNames[pickedName]);
                Module.HandleStrike();
            }
            else if (currentBird != correctBird || currentCat != correctCat)
            {
                Debug.LogFormat("[Gryphons #{0}] {5}'s types are {1}/{2} when we expected {3}/{4}, this is incorrect. Strike given.", 
                    _moduleId, birds[currentBird], cats[currentCat], birds[correctBird], cats[correctBird], gryphNames[pickedName]);
                Module.HandleStrike();
            }
            else
            {
                Debug.LogFormat("[Gryphons #{0}] {4} is wearing the {1}, and the chosen types are {2}/{3}. This is correct, module disarmed!", 
                    _moduleId, accessories[currentAccessory], birds[currentBird], cats[currentCat], gryphNames[pickedName]);
                pressedAllowed = false;
                isSolved = true;
                Module.HandlePass();
            }
        }
    }

    void refreshType()
    {

        typeBox.GetComponentInChildren<TextMesh>().text = birds[currentBird] + "/" + cats[currentCat];
    }

    void delegationZone()
    {
        catUp.OnInteract += delegate () { OnHold(); CatGoUp(); refreshType(); return false; };
        catUp.OnInteractEnded += delegate () { OnRelease(); };

        catDown.OnInteract += delegate () { OnHold(); CatGoDown(); refreshType(); return false; };
        catDown.OnInteractEnded += delegate () { OnRelease(); };


        birdUp.OnInteract += delegate () { OnHold(); BirdGoUp(); refreshType(); return false; };
        birdUp.OnInteractEnded += delegate () { OnRelease(); };

        birdDown.OnInteract += delegate () { OnHold(); BirdGoDown(); refreshType(); return false; };
        birdDown.OnInteractEnded += delegate () { OnRelease(); };


        accessoryLeft.OnInteract += delegate () { OnHold(); AccessoryGoLeft(); return false; };
        accessoryLeft.OnInteractEnded += delegate () { OnRelease(); };

        accessoryRight.OnInteract += delegate () { OnHold(); AccessoryGoRight();  return false; };
        accessoryRight.OnInteractEnded += delegate () { OnRelease(); };

        submit.OnInteract += delegate () { OnHold(); doSubmit(); return false; };
        submit.OnInteractEnded += delegate () { OnRelease(); };
    }

    

}
