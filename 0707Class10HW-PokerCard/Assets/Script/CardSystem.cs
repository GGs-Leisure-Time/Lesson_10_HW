using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//在編輯模式執行C#
[ExecuteInEditMode]
public class CardSystem : MonoBehaviour
{
    /// <summary>
    /// 52張撲克牌
    /// </summary>
    public List<GameObject> cards = new List<GameObject>();

    private string[] type = { "Spades", "Diamond", "Heart", "Club" };

    private void Start()
    {
        GetAllCard();
    }

    private void GetAllCard()
    {
        //如果撲克牌數量等於52張就跳出
        if (cards.Count == 52) return;
        //四個花色
        for (int i = 0; i < type.Length; i++)
        {
            //1-13張
            for (int j = 1; j < 14; j++)
            {
                string number = j.ToString();

                switch (j)
                {
                    case 1:
                        number = "A";
                        break;
                    case 11:
                        number = "J";
                        break;
                    case 12:
                        number = "Q";
                        break;
                    case 13:
                        number = "K";
                        break;
                }

                //卡牌 = 素材.載入<遊戲物件>("素材名稱")
                GameObject card = Resources.Load<GameObject>("PlayingCards_" + number + type[i]);
                //添加卡牌
                cards.Add(card);
            }
        }
    }

    public void ChooseCardByType(string type)
    {
        DeleteAllChild();

        //暫存牌組 = 撲克牌.哪裡((物件)=>物件.名稱.包含(花色))
        var temp = cards.Where((x) => x.name.Contains(type));

        //迴圈 遍尋 暫存牌組 生成(卡牌，父物件)
        foreach (var item in temp) Instantiate(item, transform);
        //啟動協程
        StartCoroutine(SetChildPosition());
    }
    /// <summary>
    /// 洗牌
    /// </summary>
    public void Shuffle()
    {
        DeleteAllChild();
        //另存一份洗牌用原始牌組
        List<GameObject> shuffle = cards.ToArray().ToList();
        //儲存洗牌後的新牌組
        List<GameObject> newCards = new List<GameObject>();

        for (int i = 0; i < 52; i++) 
        {
            //從洗牌用牌組隨機挑選一張
            int r = Random.Range(0, shuffle.Count);

            //挑出的隨機卡牌
            GameObject temp = shuffle[r];
            //添加到新牌組
            newCards.Add(temp);
            //刪除挑出來的牌
            shuffle.RemoveAt(r);
        }

        foreach (var item in newCards) Instantiate(item, transform);

        StartCoroutine(SetChildPosition());
    }

    /// <summary>
    /// 數字 排序:花色數字由小到大
    /// </summary>
    public void Sort()
    {
        DeleteAllChild();

        var sort = from card in cards
                   where card.name.Contains(type[3]) || card.name.Contains(type[2]) || card.name.Contains(type[1]) || card.name.Contains(type[0])
                   select card;

        foreach (var item in sort) Instantiate(item, transform);

        StartCoroutine(SetChildPosition());
    }

    /// <summary>
    /// 刪除所有子物件
    /// </summary>
    private void DeleteAllChild()
    {
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
    }
    
    /// <summary>
    /// 設定子物件座標:排序、大小、角度
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetChildPosition()
    {
        //避免刪除此次的卡牌
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < transform.childCount; i++)
        {
            //取得子物件
            Transform child = transform.GetChild(i);
            //設定角度
            child.eulerAngles = new Vector3(180, 0, 0);
            //設定尺寸
            child.localScale = Vector3.one * 20;
            //x = i%13 每13張都從1開始
            //x =(i-6)*間距
            float x = i % 13;
            //y=i/13 取得每一排的高度
            //4-y*間距
            int y = i / 13;
            child.position = new Vector3((x - 6) * 1.3f, 4-y*2, 0);

            yield return null;
        }
    }
}
