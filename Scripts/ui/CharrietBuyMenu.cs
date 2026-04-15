using Godot;

public partial class CharrietBuyMenu : Control
{
	[Export] public Godot.Collections.Array<CharrietBuyMenuOption> options;


	[ExportGroup("Connections")]

	[Export] public Button confirmButton;
	[Export] public RichTextLabel descriptionLabel;
	[Export] public VBoxContainer buttonContainer;
	[Export] public Sprite3D turretPlace;
	
	private CharrietBuyMenuOption _selectedOption;

	public CharrietBuyMenuOption SelectedOption
	{
		get => _selectedOption;
		set
		{
			_selectedOption = value;

			descriptionLabel.Text = value.description;
			turretPlace.Texture = value.thumbnail;
		}
	}


	public override void _Ready()
	{
		foreach(var option in options) AddButton(option);
	}

	public void AddButton(CharrietBuyMenuOption option)
	{
		HBoxContainer hbox = new();
		buttonContainer.AddChild(hbox);


		TextureRect thumbnail = new();

		hbox.AddChild(thumbnail);

		thumbnail.CustomMinimumSize = Vector2.One * 256;

		thumbnail.Texture = option.thumbnail;

		hbox.GuiInput += (InputEvent @event) => HandleButtonInput(@event, option);
		
	}


	public void HandleButtonInput(InputEvent @event, CharrietBuyMenuOption option)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				SelectedOption = option;
			}
		}
	}

}
