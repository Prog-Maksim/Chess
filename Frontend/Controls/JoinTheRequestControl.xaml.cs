using System.Windows.Controls;
using Frontend.Windows.Game;

namespace Frontend.Controls;

public partial class JoinTheRequestControl : UserControl
{
    private readonly string _gameId;
    
    public delegate void DeleteMenu(MenuJoinTheRequest menu);
    private readonly GameMenu.DeleteMenu _deleteMenu;
    private List<MenuJoinTheRequest> _menus = new List<MenuJoinTheRequest>();
    
    public JoinTheRequestControl()
    {
        InitializeComponent();
    }
    
    public JoinTheRequestControl(string gameId, GameMenu.DeleteMenu menu): this()
    {
        _gameId = gameId;
        _deleteMenu = menu;
    }

    public void AddPlayer(string playerName, string playerId)
    {
        DeleteMenu deleteMenu = request =>
        {
            MainStackPanel.Children.Remove(request);
            _menus.Remove(request);
            UpdateMenus();
        };
        
        MenuJoinTheRequest menu = new MenuJoinTheRequest(_menus.Count + 1, playerName, playerId, _gameId, deleteMenu);
        _menus.Add(menu);
        MainStackPanel.Children.Add(menu);
    }

    private void UpdateMenus()
    {
        for (int i = 0; i < _menus.Count; i++)
            _menus[i].UpdateCount(i + 1);
        
        if (_menus.Count == 0)
            _deleteMenu(this);
    }
}