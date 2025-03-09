using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Script;

namespace Frontend.Controls;

public partial class MenuJoinTheRequest : UserControl
{
    private string _playerId;
    private string _gameId;
    private readonly JoinTheRequestControl.DeleteMenu _deleteMenu;
    
    public MenuJoinTheRequest()
    {
        InitializeComponent();
    }

    public MenuJoinTheRequest(int count, string name, string playerId, string gameId, JoinTheRequestControl.DeleteMenu deleteMenu): this()
    {
        CountNumber.Text = count.ToString();
        NamePlayer.Text = name;
        
        _playerId = playerId;
        _gameId = gameId;
        _deleteMenu = deleteMenu;
    }

    private void Approve_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        SendRequestApprovePlayer();
    }

    private async void SendRequestApprovePlayer()
    {
        string url = Url.BaseUrl + $"Game/approve-player?gameId={_gameId}&personId={_playerId}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.ReadToken());
            
            HttpResponseMessage response = await client.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
        }
        
        _deleteMenu(this);
    }

    private void Reject_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        SendRequestRejectPlayer();
    }
    
    private async void SendRequestRejectPlayer()
    {
        string url = Url.BaseUrl + $"Game/reject-player?gameId={_gameId}&personId={_playerId}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SaveRepository.ReadToken());
            
            HttpResponseMessage response = await client.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
        }
        
        _deleteMenu(this);
    }

    public void UpdateCount(int count)
    {
        CountNumber.Text = count.ToString();
    }
}