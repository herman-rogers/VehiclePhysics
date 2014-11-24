using UnityEngine;
using System.Collections;

public class StuntAlertHandler : MonoBehaviour {
    //public UILabel stuntQuality;
    //public UILabel stuntQuantity;
    //public UILabel stunt;
    //public UILabel stuntQuantity2;
    //public UILabel stunt2;
    //public UILabel stuntQuantity3;
    //public UILabel stunt3;
    //public UILabel wipeout;
    //public UILabel pointsLabel;

    //private float overallPoints = 0;
    //private bool display;
    //private float displayTime = 2;
    //private StuntType lastStunt = StuntType.Null;
    //private int stuntnumber = 1;

    //public void DisplayStunt(StuntType stuntType)
    //{
    //    string stuntname = null;
    //    switch (stuntType){
    //        case StuntType.ForwardFlip: stuntname = "Front Flip"; break;
    //        case StuntType.BackwardFlip: stuntname = "Back Flip"; break;
    //        case StuntType.LeftRoll: stuntname = "Left Roll"; break;
    //        case StuntType.RightRoll: stuntname = "Right Roll"; break;
    //    }

    //    if(stuntType != lastStunt){
    //        stuntnumber = 1;
    //    }

    //    if (stuntType == lastStunt)
    //    {
    //        stuntnumber++;
    //        string stuntString = (stuntnumber.ToString() + "x");
    //        stuntQuantity.text = stuntString;
    //    }
    //    else if (stunt.gameObject.activeInHierarchy == false)
    //    {
    //        stunt.text = stuntname;
    //        stunt.gameObject.SetActive(true);

    //        stuntQuantity.text = (stuntnumber.ToString() + "x");
    //        stuntQuantity.gameObject.SetActive(true);
    //    }
    //    else if (stunt.gameObject.activeInHierarchy == true)
    //    {
    //        if (stunt2.gameObject.activeInHierarchy == false)
    //        {
    //            stunt2.text = stunt.text;
    //            stunt.text = stuntname;
    //            stunt2.gameObject.SetActive(true);

    //            stuntQuantity2.text = stuntQuantity.text;
    //            stuntQuantity.text = stuntnumber.ToString();
    //            stuntQuantity2.gameObject.SetActive(true);
    //        }
    //        else if (stunt2.gameObject.activeInHierarchy == true)
    //        {
    //            stunt3.text = stunt2.text;
    //            stunt2.text = stunt.text;
    //            stunt.text = stuntname;
    //            stunt3.gameObject.SetActive(true);

    //            stuntQuantity3.text = stuntQuantity2.text;
    //            stuntQuantity2.text = stuntQuantity.text;
    //            stuntQuantity.text = stuntnumber.ToString();
    //            stuntQuantity3.gameObject.SetActive(true);
    //        }
    //    }
    //    lastStunt = stuntType;
    //}

    //public void SetQuality(bool clean)
    //{
    //    if (clean)
    //    {
    //        stuntQuality.text = "Great";
    //    }
    //    else
    //    {
    //        stuntQuality.text = "Good";
    //    }
    //    stuntQuality.gameObject.SetActive(true);
    //    display = true;
    //}

    //public void clearSystem()
    //{
    //    stuntQuality.gameObject.SetActive(false);
    //    stuntQuantity.gameObject.SetActive(false);
    //    stuntQuantity2.gameObject.SetActive(false);
    //    stuntQuantity3.gameObject.SetActive(false);
    //    stunt.gameObject.SetActive(false);
    //    stunt2.gameObject.SetActive(false);
    //    stunt3.gameObject.SetActive(false);
    //    wipeout.gameObject.SetActive(false);
    //    stuntnumber = 1;
    //    lastStunt = StuntType.Null;
    //}

    //private void UpdateTimer()
    //{
    //    displayTime -= Time.deltaTime;
    //    if (displayTime < 0)
    //    {
    //        display = false;
    //        clearSystem();
    //        displayTime = 2;
    //    }
    //}



    //void Update()
    //{
    //    if (display)
    //    {
    //        UpdateTimer();
    //    }
    //}

    //public void Wipeout()
    //{
    //    //stuntQuality.text = ("[s]"+stuntQuality.text+"[/s]");
    //    //stuntQuantity.text = ("[s]" + stuntQuantity.text + "[/s]");
    //    //stuntQuantity2.text = ("[s]" + stuntQuantity2.text + "[/s]");
    //    //stuntQuantity3.text = ("[s]" + stuntQuantity3.text + "[/s]");
    //    //stunt.text = ("[s]"+stunt.text+"[/s]");
    //    //stunt2.text = ("[s]" + stunt2.text + "[/s]");
    //    //stunt3.text = ("[s]" + stunt3.text + "[/s]");
    //    clearSystem();
    //    wipeout.gameObject.SetActive(true);
    //    display = true;
    //}

    //public void UpdateHighScore(float points)
    //{
    //    overallPoints += points;
    //    pointsLabel.text = overallPoints.ToString();
    //}

}
