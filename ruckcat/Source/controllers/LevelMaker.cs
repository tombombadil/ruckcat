using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ruckcat
{

    [Serializable]
    public class LevelMakerItem
    {
        public enum ItemInsertType { SPACE, ADD_TO_PREV }; //space=prefablarin sizelarindan bagimsiz dizilim, ADD_TO_PREV=bir onceki prefabin ucuna ekle
        public ItemInsertType InsertType; //spacemin ve spacemax in nasil kullanacagini belirliyor
        [Tooltip("Space tipinde toplam aralık iken add_to_prev de objeler arası boşluk")]
        //public int Count = 1; //spawn edilecek item sayisi
        public int StartIndex = 0;  //PrefabsLinear'dan hangi indexten baslanacak. ilk seferde belirlenir. sonraki repeatlerde dikkate alinmaz.
        public ListElement[] Elements;
        public GameObject PrefabEnd; //son element
                                     // public int LinearRepeat = 1; //LinearRepeat -> linear liste kaz kez tekrar etsin. Count > PrefabsLinear.count if sarti gerekli.
        public Vector3 SpawnDirection;
        public Vector3 SpaceMin;
        public Vector3 SpaceMax; //Random vektörler objelerin son noktasına rastgele gelen sayı eklenir
        public GameObject ReferenceForLimit; //limitlendirmeyi hangi objeye gore yapacak?
        public Vector3 Limits; // limit yapilacak axis'i 1 yapmak gerek
        [NonSerialized]
        public Vector3 LimitDistance; // limit yapilacak axis'i 1 yapmak gerek


    }

    [Serializable]
    public class ListElement
    {
        public GameObject[] Prefabs;
        public bool isRandom;
        public int Count;
    }

    public class LevelMaker : CoreCont<LevelMaker>
    {
        [Serializable]
        public class LevelData
        {
            public int Level = 0;
            public LevelMakerItem[] List;
        }

        public LevelData[] Levels;
        [NonSerialized]
        public int levelcount;
        private Vector3 spawnDir = new Vector3(0, 0, 0);
        private Vector3 boundsHolder;
        private Vector3 firstPosition;
        private bool isFirst = true;
        private bool isInitiated;
        private bool isLimited = false;
        private Vector3 distance = Vector3.zero;
        GameObject goPointer;

        bool initCont;
        public override void Init()
        {
            base.Init();
            if (Levels.Length == 0) return;
            getLevel();
            for (int i = 0; i < Levels[levelcount].List.Length; i++)
            {

                if (Levels[levelcount].List[i].ReferenceForLimit)
                {
                    initCont = true;
                    break;
                }
            }
            if (!initCont)
            {
                if (!isInitiated)
                {
                    clearChilds();
                    levelmakerinit();
                    isInitiated = true;
                }
                for (int i = 0; i < Levels[levelcount].List.Length; i++)
                {
                    foreach (LevelMaker go in FindObjectsOfType<LevelMaker>() as LevelMaker[])
                    {
                        if (gameObject != go)
                        {
                            go.getLevel();
                            for (int j = 0; j < go.Levels[go.levelcount].List.Length; j++)
                            {
                                if (go.Levels[go.levelcount].List[j].ReferenceForLimit == gameObject)
                                {
                                    go.refinit(j, go);
                                }
                                else if (go.Levels[go.levelcount].List[j].ReferenceForLimit == null && go.isInitiated == false)
                                {
                                    go.refinit(j, go);
                                }
                            }
                            go.isInitiated = true;

                        }
                    }

                }
            }

        }

        public void levelmakerinit()
        {
            clearChilds();
            getLevel();
            goPointer = new GameObject("goPointer");

            for (int i = 0; i < Levels[levelcount].List.Length; i++)
            {

                firstPosition = gameObject.transform.position;
                gameObject.transform.position = Vector3.zero;
                goPointer.transform.position = gameObject.transform.position;
                generateLevel(Levels[levelcount].List[i]);
            }
        }

        public void refinit(int i, LevelMaker go)
        {
            checkDestination(i, go);
            goPointer = new GameObject("goPointer");
            firstPosition = gameObject.transform.position;
            gameObject.transform.position = Vector3.zero;
            goPointer.transform.position = gameObject.transform.position;
            generateLevel(Levels[levelcount].List[i]);
        }

        public void checkDestination(int i, LevelMaker go)
        {
            if (go.Levels[levelcount].List[i].ReferenceForLimit != null)
            {
                go.Levels[levelcount].List[i].LimitDistance = new Vector3(
                             go.Levels[levelcount].List[i].ReferenceForLimit.GetComponent<LevelMaker>().distance.x * go.Levels[levelcount].List[i].Limits.x,
                             go.Levels[levelcount].List[i].ReferenceForLimit.GetComponent<LevelMaker>().distance.y * go.Levels[levelcount].List[i].Limits.y,
                             go.Levels[levelcount].List[i].ReferenceForLimit.GetComponent<LevelMaker>().distance.z * go.Levels[levelcount].List[i].Limits.z);
            }
        }



        public void getLevel()          //Level seçimini yapan yer eğer seçtiğimiz level var ise onu alır eğer geldiğimiz level ayarlanmamışsa ayarlanmıs onun altında bulunan en son ayarı alır.
        {
            int tempLevelcount = -1;
            levelcount = HyperLevelCont.Instance.CurrLevel;
            for (int i = 0; i < Levels.Length; i++)
            {
                if (Levels[i].Level == levelcount) tempLevelcount = i;
            }
            if (tempLevelcount == -1)
            {
                for (int i = 0; i < Levels.Length; i++)
                {
                    if (Levels[i].Level < levelcount && Levels[i].Level > tempLevelcount) tempLevelcount = i;
                }
                levelcount = tempLevelcount;
            }
            else levelcount = tempLevelcount;

        }



        public void generateLevel(LevelMakerItem levelDatas)            //Levelı oluşturan yer ilk olarak indexi alıp 1. linear repeati bitirir sonrasında repeat var ise onu yoksa random prefaba gider
        {                                                                              // Random prefab listeside yok ise linearin son itemini count sayısına kadar almaya devam eder Eğer FinishPrefab var ise toplam sayının 1 
            if (levelDatas.Elements.Length == 0) erorLogs(3);   //eksiğini alıp sona finishprefabi ekler
            //if (levelDatas.LinearRepeat < 1) erorLogs(1);
            int tempCounter = 0;
            int tempindex = levelDatas.StartIndex;
            int finishCont = 0;
            if (levelDatas.PrefabEnd) finishCont = 1;
            isLimited = false;
            distance = Vector3.zero;

            int totalCount = 0;
            foreach (var item in levelDatas.Elements)
            {
                totalCount += item.Count;
            }

            for (int i = 0; i < levelDatas.Elements.Length; i++)
            {
                int levelspawncount = 0;
                bool islastSpawn = false;
                if (levelDatas.Elements[i].Count + tempCounter + finishCont >= totalCount)
                {
                    levelspawncount = totalCount - tempCounter;
                    islastSpawn = true;
                }


                if (i == 0)
                {
                    if (!levelDatas.Elements[i].isRandom) GenerateElements(levelDatas.Elements[i], levelDatas, tempindex, levelspawncount);
                    else GenerateElements(levelDatas.Elements[i], levelDatas, 0, levelspawncount);
                    tempCounter -= tempindex;
                }
                else GenerateElements(levelDatas.Elements[i], levelDatas, 0, levelspawncount);

                tempCounter += levelDatas.Elements[i].Count;
                if (islastSpawn) break;
            }


            if (finishCont == 1 && !isLimited) spawnitemFinish(levelDatas);
            gameObject.transform.position = firstPosition;
            isFirst = true;
            levelDatas.LimitDistance = distance;
            Destroy(goPointer);
        }

        public void GenerateElements(ListElement _elements, LevelMakerItem _ldatas, int startindex = 0, int spawncount = 0)
        {
            int sCount = _elements.Count;
            if (spawncount > 0) sCount = spawncount;
            if (startindex > 0) sCount += startindex;

            for (int i = startindex; i < sCount; i++)
            {
                int snum = i % _elements.Prefabs.Length;

                if (!_elements.isRandom) spawnitemL(_ldatas, _elements.Prefabs[snum]);
                else
                {
                    int randnum = UnityEngine.Random.Range(0, _elements.Prefabs.Length);
                    spawnitemL(_ldatas, _elements.Prefabs[randnum]);
                }
            }
        }

        public void spawnitemL(LevelMakerItem levelDatas, GameObject _item)   // linear random ve finish prefabları oluşturan metodlar
        {
            calculateSpawnPose(levelDatas, _item);
            CoreSceneCont.Instance.SpawnItem<CoreSceneObject>(_item, goPointer.transform.position, transform);
            calculateSpawnPose(levelDatas, _item);
        }
        public void spawnitemFinish(LevelMakerItem levelDatas)
        {
            calculateSpawnPose(levelDatas, levelDatas.PrefabEnd);
            CoreSceneCont.Instance.SpawnItem<CoreSceneObject>(levelDatas.PrefabEnd, goPointer.transform.position, transform);
            calculateSpawnPose(levelDatas, levelDatas.PrefabEnd);
        }

        public void calculateSpawnPose(LevelMakerItem levelDatas, GameObject goObj)     // Spawn yapılacak pozisyonu ayarlayan metod
        {
            spawnDir = levelDatas.SpawnDirection;
            relocatePointer(new Vector3((calculateBounds(goObj).x / 2) * spawnDir.x, (calculateBounds(goObj).y / 2) * spawnDir.y,
                (calculateBounds(goObj).z / 2) * spawnDir.z), levelDatas, goObj);
        }

        public Vector3 calculateBounds(GameObject goParent)     // Prefabin kapladığı alanı bulan metod
        {
            Quaternion currentRotation = this.transform.rotation;
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            Bounds bounds = new Bounds(this.transform.position, Vector3.zero);

            foreach (Renderer renderer in goParent.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            Vector3 localCenter = bounds.center - this.transform.position;
            bounds.center = localCenter;

            boundsHolder = bounds.center * 2;

            this.transform.rotation = currentRotation;
            return new Vector3(bounds.size.x - (bounds.center.x * 2), bounds.size.y - (bounds.center.y * 2), bounds.size.z - (bounds.center.z * 2));
        }


        int tempR = 0;
        Vector3 tempvecholder = new Vector3(0, 0, 0);
        Vector3 tempposvecholder = new Vector3(0, 0, 0);
        public void relocatePointer(Vector3 plusVec, LevelMakerItem leveldatas, GameObject go)
        {
            float tempXPose = 0;
            float tempYPose = 0;
            float tempZPose = 0;

            goPointer.transform.position += plusVec;
            tempvecholder += plusVec;
            tempR++;

            if (tempR % 2 == 1 && !isFirst)
            {
                if (spawnDir.x == 0) tempXPose += UnityEngine.Random.Range(leveldatas.SpaceMin.x, leveldatas.SpaceMax.x);
                if (spawnDir.y == 0) tempYPose += UnityEngine.Random.Range(leveldatas.SpaceMin.y, leveldatas.SpaceMax.y);
                if (spawnDir.z == 0) tempZPose += UnityEngine.Random.Range(leveldatas.SpaceMin.z, leveldatas.SpaceMax.z);
                tempposvecholder = new Vector3(tempXPose, tempYPose, tempZPose);
                goPointer.transform.position += tempposvecholder;
            }
            if (isFirst == true)
            {
                if (leveldatas.SpawnDirection.x != 0) goPointer.transform.position -= new Vector3((calculateBounds(go).x / 2) * spawnDir.x, 0, 0);
                if (leveldatas.SpawnDirection.y != 0) goPointer.transform.position -= new Vector3(0, (calculateBounds(go).y / 2) * spawnDir.y, 0);
                isFirst = false;
            }

            if (tempR % 2 == 0)
            {
                Vector3 tempRandVec = new Vector3(
                    UnityEngine.Random.Range(leveldatas.SpaceMin.x, leveldatas.SpaceMax.x),
                    UnityEngine.Random.Range(leveldatas.SpaceMin.y, leveldatas.SpaceMax.y),
                    UnityEngine.Random.Range(leveldatas.SpaceMin.z, leveldatas.SpaceMax.z));

                goPointer.transform.position -= tempposvecholder;


                goPointer.transform.position += new Vector3(boundsHolder.x * spawnDir.x, boundsHolder.y * spawnDir.y, boundsHolder.z * spawnDir.z);
                tempvecholder += new Vector3(boundsHolder.x * spawnDir.x, boundsHolder.y * spawnDir.y, boundsHolder.z * spawnDir.z);

                switch ((int)leveldatas.InsertType)
                {
                    case 0: //space ise
                        goPointer.transform.position += new Vector3(
                            spawnDir.x * tempRandVec.x,
                            spawnDir.y * tempRandVec.y,
                            spawnDir.z * tempRandVec.z) - tempvecholder;
                        break;
                    case 1: //add_to_prev ise
                        goPointer.transform.position += new Vector3(
                            spawnDir.x * tempRandVec.x,
                            spawnDir.y * tempRandVec.y,
                            spawnDir.z * tempRandVec.z);
                        break;
                }
                distance += (new Vector3(tempvecholder.x * spawnDir.x, tempvecholder.y * spawnDir.y, tempvecholder.z * spawnDir.z) +
                    new Vector3(spawnDir.x * tempRandVec.x, spawnDir.y * tempRandVec.y, spawnDir.z * tempRandVec.z));
                if (distance.x * spawnDir.x >= leveldatas.LimitDistance.x && distance.y * spawnDir.y >= leveldatas.LimitDistance.y && distance.z * spawnDir.z >= leveldatas.LimitDistance.z)
                    if (leveldatas.Limits.x > 0 || leveldatas.Limits.y > 0 || leveldatas.Limits.z > 0) isLimited = true;
                tempvecholder = new Vector3(0, 0, 0);
            }
        }

        public void clearChilds()
        {
            Transform[] childs = gameObject.GetComponentsInChildren<Transform>();
            if (childs.Length > 0)
            {
                foreach (var child in childs)
                {
                    if (child != gameObject.transform) Destroy(child.gameObject);
                }
            }

        }


        public void erorLogs(int erorType)
        {
            switch (erorType)
            {
                case 0:
                    Debug.LogWarning("Oluşturduğun levellar arasında bu levela ait veri yok en uygun önceki levela ait veriler alındı");
                    break;
                case 1:
                    Debug.LogError("Linear Repeat 1 den az olamaz");
                    break;
                case 2:
                    Debug.LogError("Linear repeat ve Linearprefab sayısı toplam objeyi geçmiyor ise Randomprefab boş olamaz");
                    break;
                case 3:
                    Debug.LogError("Linear ve Random Prefab aynı anda boş olamaz");
                    break;
            }
        }




    }
}