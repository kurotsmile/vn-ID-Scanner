
using System.Collections;
using Carrot;
using SimpleFileBrowser;
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
    public GameObject item_list_prefab;
    public Transform tr_arean_list_main;

    public Color32 color_a;
    public Color32 color_b;
    public IList list_data_cccd;
    
    void Start()
    {
        this.carrot.Load_Carrot();
        this.panel_main.SetActive(true);
        this.panel_scaner.SetActive(false);
        this.list_data_cccd=(IList) Json.Deserialize("[]");
        CodeReader.OnCodeFinished += getDataFromReader;
        this.Update_list_data();
    }

    void Update()
    {

    }

    public void Btn_show_qr_scaner(){
        this.carrot.play_sound_click();
        this.panel_main.SetActive(false);
        this.panel_scaner.SetActive(true);
        reader.StartWork();
        this.carrot.delay_function(2f,()=>{
            reader.StartWork();
        });
    }

    public void Btn_close_qr_scanner(){
        this.carrot.play_sound_click();
        this.panel_scaner.SetActive(false);
        this.panel_main.SetActive(true);
        reader.StopWork();
    }

    public void getDataFromReader(string dataStr)
	{
        this.Add_data(dataStr);
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
        string path=s_paths[0];
        string s_data="";
        foreach (string row in this.list_data_cccd)
        {
           string formattedRow = row.Replace("|", ",");
            s_data+=formattedRow+"\n";
        }
        FileBrowserHelpers.WriteTextToFile(s_paths[0],s_data);
        this.carrot.Show_msg("Data export","Data export successful at path "+path+" !");
    }

    [ContextMenu("Test Add Data")]
    public void Test_add_data(){
        this.Add_data(""+Random.Range(1,9)+""+Random.Range(1,9)+""+Random.Range(1,9)+"|000|Tran Thien Thanh");
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
        }else{
            for(int i=0;i<this.list_data_cccd.Count;i++){
                var index_cccd=i;
                string[] arr_s=this.list_data_cccd[i].ToString().Split("|");
                Carrot_Box_Item item_m=this.Add_item_m();
                item_m.set_title(arr_s[0]);
                item_m.set_tip(arr_s[2]);
                item_m.set_icon(this.carrot.icon_carrot_database);

                if(i%2==0)
                    item_m.GetComponent<Image>().color=this.color_a;
                else
                    item_m.GetComponent<Image>().color=this.color_b;

                Carrot_Box_Btn_Item btn_del=item_m.create_item();
                btn_del.set_icon(this.carrot.sp_icon_del_data);
                btn_del.set_icon_color(Color.white);
                btn_del.set_color(this.carrot.color_highlight);
                btn_del.set_act(()=>{
                    this.list_data_cccd.RemoveAt(index_cccd);
                    this.Update_list_data();
                });
            }
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
}
