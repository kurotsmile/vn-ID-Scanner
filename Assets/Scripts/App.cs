using System;
using System.Collections;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Main Obj")]
    public Carrot.Carrot carrot;
    public CodeReader reader;
    public Carrot_File file;

    [Header("App Obj")]
    
    public GameObject panel_main;
    public GameObject panel_scaner;

    public Text txt_show_data;
    public IList list_data_cccd;
    
    void Start()
    {
        this.carrot.Load_Carrot();
        this.panel_main.SetActive(true);
        this.panel_scaner.SetActive(false);
        CodeReader.OnCodeFinished += getDataFromReader;
    }

    void Update()
    {

    }

    public void Btn_show_qr_scaner(){
        this.carrot.play_sound_click();
        this.panel_main.SetActive(false);
        this.panel_scaner.SetActive(true);
        reader.StartWork();
    }

    public void Btn_close_qr_scanner(){
        this.carrot.play_sound_click();
        this.panel_scaner.SetActive(false);
        this.panel_main.SetActive(true);
        reader.StopWork();
    }

    public void getDataFromReader(string dataStr)
	{
        this.txt_show_data.text=dataStr;
        this.carrot.play_sound_click();
        this.panel_scaner.SetActive(false);
        this.panel_main.SetActive(true);
        reader.StopWork();
	}

    public void Btn_export_exel(){
        this.file.Set_filter(Carrot_File_Data.ExelData);
        this.file.Save_file(Done_export_exel);
    }

    private void Done_export_exel(string[] s_paths){
        
    }
}
