using Godot;

public class FolderLineEdit : HBoxContainer
{
    public LineEdit leFolder;
    public LineEdit leProduct;

    public override void _Ready()
    {
        leFolder = GetNode<LineEdit>("LEF");
        leProduct = GetNode<LineEdit>("LEP");
    }

    void OnLEFChanged(string new_text) {

        if (GetIndex() == GetParent().GetChildCount()-1 ) {
            Node dp = this.Duplicate();
            GetParent().AddChild(dp);
            (dp as FolderLineEdit).leFolder.Text = "";
        }
        leProduct.Text = leFolder.Text.GetFile();
    }

    void OnBtnDeletePressed() {
        if (!(GetIndex() == GetParent().GetChildCount()-1)) {
            QueueFree();
            Main.instance.StartUpdateTimer();
        }
    }

}