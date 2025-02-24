
using System;
using System.Collections.Generic;
using System.IO;
using Carrot;
using MTAssets.NativeAndroidToolkit;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Main Obj")]
    public Carrot.Carrot carrot;

    [Header("App Obj")]
    public GameObject item_list_prefab;
    public Transform tr_arean_list_main;

    public Color32 color_a;
    public Color32 color_b;
    public GameObject obj_delete_all_btn;
    public List<String> list_data_cccd;
    
    void Start()
    {
        if (NativeAndroidToolkit.isInitialized == false) NativeAndroidToolkit.Initialize();

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
            Permission.RequestUserPermission(Permission.Camera);
        }

        this.carrot.Load_Carrot();
        this.list_data_cccd=new List<string>();
        this.Update_list_data();
    }

    public void Btn_show_qr_scaner(){
        this.carrot.play_sound_click();
        //Check if the device can open the Camera
        if (NAT.Camera.isCameraSupported() == true)
        {
            //First, register the callbacks...

            //- onCameraReadCode(codeReaded)
            NATEvents.onCameraReadCode += (string result) =>
            {
                getDataFromReader(result);
                Debug.Log("The result of Scanned Code is " + result);
            };
            //- onCameraClose()
            NATEvents.onCameraClose += () =>
            {
                Debug.Log("The Camera was closed before read a code.");
            };

            new NAT.Camera.CameraNative(NAT.Camera.CameraMode.CodeReader, ScreenOrientation.AutoRotation)
                .setTitle("Code Reader")
                .setCodeReaderMessage("Point to a Bar/QR Code and center it on the Screen")
                .setEnableFlash(true)
                .setEnableSwitch(true)
                .setDefaultCamera(NAT.Camera.CameraType.BackCamera)
                .OpenThisCamera();
        }
        if (NAT.Camera.isCameraSupported() == false)
            Debug.LogError("Camera is not supported on this device!");
    }


    public void getDataFromReader(string dataStr)
	{
        Screen.orientation = ScreenOrientation.Portrait;
        this.carrot.delay_function(2f,()=>{
            Screen.orientation = ScreenOrientation.Portrait;
        });
        this.Add_data(dataStr);
        this.carrot.play_sound_click();
	}

    public void Btn_export_exel(){
        //this.file.Set_filter(Carrot_File_Data.ExelData);
        //this.file.Save_file(Done_export_exel);

        string s_data="";
        s_data="số CCCD,Số CMND,tên,ngày sinh,giới tính,nơi cư trú,ngày cấp\n";
        for(int i=0;i<this.list_data_cccd.Count;i++){
            string[] arr_s=this.list_data_cccd[i].ToString().Split("|");
            s_data +="\""+arr_s[0]+"\",";
            s_data +="\""+arr_s[1]+"\",";
            s_data +="\""+arr_s[2]+"\",";
            s_data +="\""+arr_s[3]+"\",";
            s_data +="\""+arr_s[4]+"\",";
            s_data +="\""+arr_s[5]+"\",";
            s_data +="\""+arr_s[6]+"\"";
            s_data+="\n";
        }

        NATEvents.onFilesFilePickerOperationFinished += (NAT.Files.FilePickerOperationStatus status, NAT.Files.FilePickerOperationResponse response) =>
        {
            if (status == NAT.Files.FilePickerOperationStatus.Successfully)
                if (response.operationType == NAT.Files.FilePickerAction.CreateFile)
                {
                    NAT.Files.WriteAllText(response.scopeOfOperation,Path.GetDirectoryName(response.pathFromOperationInScope), Path.GetFileName(response.pathFromOperationInScope), s_data);
                    Debug.Log("The file \"" + Path.GetFileName(response.pathFromOperationInScope) + "\" was saved!");
                    this.carrot.Show_msg("Data export", "Data export successful at path " + Path.GetFileName(response.pathFromOperationInScope) + " !");
                }

            if (status == NAT.Files.FilePickerOperationStatus.InvalidScope)
                NAT.Dialogs.ShowSimpleAlertDialog("File Picker Operation Result", "Invalid scope! Please select another location!", false);
            if (status == NAT.Files.FilePickerOperationStatus.Canceled)
                NAT.Dialogs.ShowSimpleAlertDialog("File Picker Operation Result", "The File Picker operation has been canceled by the user!", false);
            if (status == NAT.Files.FilePickerOperationStatus.Unknown)
                NAT.Dialogs.ShowSimpleAlertDialog("File Picker Operation Result", "An internal error occurred in File Picker!", false);
        };
        
        new NAT.Files.FilePickerOperation(NAT.Files.FilePickerInterfaceMode.PortraitFullscreen, "Select a Place To Create File")
            .setCreateFileOperation(NAT.Files.FilePickerDefaultScope.Documents, "cccd.csv")
            .setMimeType(NAT.Files.MimeType.CSV)
            .OpenFilePicker();
    }

    [ContextMenu("Test Add Data")]
    public void Test_add_data(){
        this.Add_data("11111223|0001111|Trần Thiện Thanh|28091993|Nam|Duong son,huong toan,hue|3082024");
        this.Update_list_data();
    }

    public void Add_data(string s_data){
        this.list_data_cccd.Add(s_data);
        this.Update_list_data();
    }

    public void Update_list_data(){
        this.carrot.clear_contain(this.tr_arean_list_main);
        if(this.list_data_cccd.Count==0){
            Carrot_Box_Item item_none=this.Add_item_m();
            item_none.set_icon(this.carrot.icon_carrot_all_category);
            item_none.set_title("Empty list");
            item_none.set_tip("Start scanning the qr cccd code to add data");
            item_none.set_act(this.Btn_show_qr_scaner);
            this.obj_delete_all_btn.SetActive(false);
        }else{
            this.obj_delete_all_btn.SetActive(true);
            for(int i=0;i<this.list_data_cccd.Count;i++){
                var index_cccd=i;
                string[] arr_s=this.list_data_cccd[i].ToString().Split("|");
                var arr_data=arr_s;
                Carrot_Box_Item item_m=this.Add_item_m();
                item_m.set_title(arr_s[0]);
                item_m.set_tip(arr_s[2]);
                item_m.set_icon(this.carrot.icon_carrot_database);

                if(i%2==0)
                    item_m.GetComponent<Image>().color=this.color_a;
                else
                    item_m.GetComponent<Image>().color=this.color_b;

                Carrot_Box_Btn_Item btn_view=item_m.create_item();
                btn_view.set_icon(this.carrot.icon_carrot_visible_off);
                btn_view.set_icon_color(Color.white);
                btn_view.set_color(this.carrot.color_highlight);
                btn_view.set_act(()=>{this.Show_info_data(arr_data);});

                Carrot_Box_Btn_Item btn_del=item_m.create_item();
                btn_del.set_icon(this.carrot.sp_icon_del_data);
                btn_del.set_icon_color(Color.white);
                btn_del.set_color(this.carrot.color_highlight);
                btn_del.set_act(()=>{
                    this.list_data_cccd.RemoveAt(index_cccd);
                    this.Update_list_data();
                });

                item_m.set_act(()=>{
                    this.Show_info_data(arr_data);
                });
            }
        }
    }

    private void Show_info_data(string[] arr_data){
        this.carrot.play_sound_click();
        Carrot_Box box_info=this.carrot.Create_Box();
        box_info.set_icon(this.carrot.user.icon_user_info);
        box_info.set_title("Info");

        if(arr_data[0]!=null){
            Carrot_Box_Item m_item_cccd=box_info.create_item();
            m_item_cccd.set_icon(this.carrot.icon_carrot_nomal);
            m_item_cccd.set_title("CCCD No");
            m_item_cccd.set_tip(arr_data[0]);
        }

        if(arr_data[1]!=null){
            Carrot_Box_Item m_item_cccd=box_info.create_item();
            m_item_cccd.set_icon(this.carrot.icon_carrot_nomal);
            m_item_cccd.set_title("CMND No");
            m_item_cccd.set_tip(arr_data[1]);
        }

        if(arr_data[2]!=null){
            Carrot_Box_Item m_item_cccd=box_info.create_item();
            m_item_cccd.set_icon(this.carrot.user.icon_user_login_true);
            m_item_cccd.set_title("Full Name");
            m_item_cccd.set_tip(arr_data[2]);
        }

        if(arr_data[3]!=null){
            Carrot_Box_Item m_item_cccd=box_info.create_item();
            m_item_cccd.set_icon(this.carrot.sp_icon_table_color);
            m_item_cccd.set_title("Date of birth");
            m_item_cccd.set_tip(arr_data[3]);
        }

        if(arr_data[4]!=null){
            Carrot_Box_Item m_item_cccd=box_info.create_item();
            m_item_cccd.set_icon(this.carrot.sp_icon_table_color);
            m_item_cccd.set_title("Sex");
            m_item_cccd.set_tip(arr_data[4]);
        }

         if(arr_data[5]!=null){
            Carrot_Box_Item m_item_cccd=box_info.create_item();
            m_item_cccd.set_icon(this.carrot.icon_carrot_location);
            m_item_cccd.set_title("Address");
            m_item_cccd.set_tip(arr_data[5]);
        }

         if(arr_data[6]!=null){
            Carrot_Box_Item m_item_cccd=box_info.create_item();
            m_item_cccd.set_icon(this.carrot.icon_carrot_all_category);
            m_item_cccd.set_title("Date of issue");
            m_item_cccd.set_tip(arr_data[6]);
        }
    }

    private Carrot_Box_Item Add_item_m(){
        GameObject obj_m=Instantiate(this.item_list_prefab);
        obj_m.transform.SetParent(this.tr_arean_list_main);
        obj_m.transform.localScale=new Vector3(1f,1f,1f);
        Carrot_Box_Item item_m=obj_m.GetComponent<Carrot_Box_Item>();
        item_m.set_type(Box_Item_Type.box_nomal);
        return item_m;
    }

    public void Btn_delete_all_data(){
        this.carrot.play_sound_click();
        this.list_data_cccd=new List<string>();
        this.Update_list_data();
    }
}
