using Godot;

public class FolderLineEdit : Node
{
    public LineEdit leFolder;
    public LineEdit leProduct;

    public override void _Ready()
    {
        leFolder = GetNode<LineEdit>("LEF");
        leProduct = GetNode<LineEdit>("LEP");
    }

    /// <summary> Add a Line when the User enters Text and set the ProductName to the folders name </summary>
    void OnLEFChanged(string new_text) // Run when the user Enters Text
    {

        if (GetIndex() == GetParent().GetChildCount() - 1)
        {
            Node dp = this.Duplicate();
            GetParent().AddChild(dp);
            (dp as FolderLineEdit).leFolder.Text = "";
        }
        leProduct.Text = leFolder.Text.GetFile();
    }

    void OnBtnDeletePressed()
    {
        if (!(GetIndex() == GetParent().GetChildCount() - 1))
        {
            QueueFree();
            Main.instance.StartUpdateTimer();
        }
    }
}