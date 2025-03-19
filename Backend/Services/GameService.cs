using Backend.Enums;
using Backend.Game;
using Backend.Game.GameModes;
using Backend.Game.GameModes.Blitz;
using Backend.Game.GameModes.Bullet;
using Backend.Game.GameModes.Classical;
using Backend.Game.GameModes.Rapid;
using Backend.Game.Shapes;
using Backend.Models.Response;
using Backend.Repository.Interfaces;

namespace Backend.Services;

public class GameService
{
    /// <summary>
    /// Список всех активных игр
    /// </summary>
    private List<BaseChessGame> GetAllGames { get; set; }
    
    private readonly Lazy<SendWebSocketMessage> _sendWebSocketMessage;
    private readonly IUserRepository _userRepository;
    private readonly PlayerDataService _playerDataService;

    public delegate void DeleteGame(string gameId);
    
    public GameService(Lazy<SendWebSocketMessage> sendWebSocketMessage, IUserRepository userRepository, PlayerDataService playerDataService)
    {
        GetAllGames = new List<BaseChessGame>();
        _sendWebSocketMessage = sendWebSocketMessage;
        _userRepository = userRepository;
        _playerDataService = playerDataService;
    }

    /// <summary>
    /// Метод создания игры
    /// </summary>
    /// <param name="nameGame">Название игры</param>
    /// <param name="players">Кол-во игроков</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <param name="name">Никнейм пользователя</param>
    /// <param name="isPrivate">Приватная ли игра</param>
    /// <param name="isPotion">Доступны ли зелья в игре</param>
    /// <param name="gameMode">Тип игры</param>
    /// <returns>Идентификатор игры</returns>
    public string CreateGame(string nameGame, int players, string playerId, string name, bool isPrivate, bool isPotion, GameMode gameMode)
    {
        DeleteGame deleteGame = DeleteGameHandler;
        IGameMode mode = GetGameSettings(gameMode, players);

        var potions = GenerateRandomPotions(isPotion);
        ChessPlayer player = new ChessPlayer(playerId, name, _sendWebSocketMessage.Value, mode, potions);

        if (players == 2)
        {
            ChessGame2Players chessGame2Players = new ChessGame2Players(nameGame, player, isPotion, mode, isPrivate, _sendWebSocketMessage, deleteGame, _userRepository, _playerDataService);
            GetAllGames.Add(chessGame2Players);
            return chessGame2Players.GameId;
        }
        
        ChessGame4Players chessGame4Players = new ChessGame4Players(nameGame, player, isPotion, mode, isPrivate, _sendWebSocketMessage, deleteGame, _userRepository, _playerDataService);
        GetAllGames.Add(chessGame4Players);
        return chessGame4Players.GameId;
    }

    private IGameMode GetGameSettings(GameMode mode, int playerCount)
    {
        if (mode == GameMode.Classic)
        {
            if (playerCount == 2)
                return new Classical2Player();
            if (playerCount == 4)
                return new Classical4Player();
        }
        if (mode == GameMode.Rapid)
        {
            if (playerCount == 2)
                return new Rapid2Player();
            if (playerCount == 4)
                return new Rapid4Player();
        }
        if (mode == GameMode.Blitz)
        {
            if (playerCount == 2)
                return new Blitz2Player();
            if (playerCount == 4)
                return new Blitz4Player();
        }
        if (mode == GameMode.Bullet)
        {
            if (playerCount == 2)
                return new Bullet2Player();
            if (playerCount == 4)
                return new Bullet4Player();
        }

        throw new NullReferenceException();
    }

    private void DeleteGameHandler(string gameId)
    {
        var game = GetAllGames.FirstOrDefault(g => g.GameId == gameId);
        
        if (game != null)
            GetAllGames.Remove(game);
        
        Console.WriteLine($"Игра: {gameId} была удалена!");
    }

    /// <summary>
    /// Оставляет заявку на вступления в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <param name="nickname">Никнейм пользователя</param>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<BaseResponse> JoinGame(string gameId, string playerId, string nickname)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);
        
        if (game == null)
            return new BaseResponse { Message = "Данная игра не найдена", Error = "NotFound", StatusCode = 404 };
        
        ChessPlayer? searchPlayer = game.Players.FirstOrDefault(p => p.Id == playerId);
        ChessPlayer player;
        
        player = searchPlayer ?? new ChessPlayer(playerId, nickname, _sendWebSocketMessage.Value, game.Mode, GenerateRandomPotions(game.IsPotion));
        
        await game.RequestJoin(player);
        return new BaseResponse
        {
            StatusCode = 200,
            Success = true,
            Message = "Заявка на вступление в игру оставлена! ожидайте"
        };
    }

    /// <summary>
    /// Генерация зелий
    /// </summary>
    /// <param name="isPermission"></param>
    /// <returns></returns>
    private List<PotionType> GenerateRandomPotions(bool isPermission)
    {
        if (!isPermission)
            return new List<PotionType>();

        int countPotion = 5;
        Random random = new Random();
        var allPotions = Enum.GetValues(typeof(PotionType)).Cast<PotionType>().ToList();
        var potions = allPotions.OrderBy(_ => random.Next()).Take(countPotion).ToList();
        return potions;
    }

    /// <summary>
    /// Возвращает информацию об игре
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    /// <exception cref="NullReferenceException">Данный игрок не найден</exception>
    /// <exception cref="UnauthorizedAccessException">У данного игрока нет доступа к игре</exception>
    public async Task<GameData> GetBoard(string gameId, string playerId, PersonData potionData)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
        {
            throw new KeyNotFoundException("Данная игра не найдена");
        }
        
        var player = game.Players.Find(x => x.Id == playerId);

        if (player == null)
            throw new NullReferenceException("Данный игрок не найден");
        
        if (game.IsGamePrivate && !player.IsApproved)
            throw new UnauthorizedAccessException("У вас нет доступа к этой игре");
        
        PotionAvailable potionAvailable = new PotionAvailable
        {
            Potions = new List<PotionDataAvailable>()
        };

        if (game.IsPotion)
        {
            var potions = potionData.Potions.Where(x => player.AvailablePotion.Contains(x.Type)).ToList();
            
            foreach (var potion in potions)
            {
                PotionDataAvailable potionDataAvailable = new PotionDataAvailable
                {
                    Type = potion.Type,
                    PotionId = potion.PotionId,
                    IsUnlocked = potion.IsUnlocked,
                    IsAvailable = potion.IsUnlocked && potion.Count > 0
                };
                potionAvailable.Potions.Add(potionDataAvailable);
            }
        }
        
        GameData data = new GameData
        {
            PersonId = playerId,
            GameId = gameId,
            PotionAvailable = game.IsPotion? potionAvailable: null,
            GameName = game.GameName,
            GameState = game.State,
            Score = player.Score,
            Moves = game.Moves,
            KillPiece = player.GetListKillPiece(),
            CurrentPlayer = game.Players[game.CurrentPlayerIndex].Id
        };
        data.Players = new List<GamePlayer>();

        if (game.OwnerId == playerId)
        {
            List<GamePlayer> players = new List<GamePlayer>();

            foreach (var chessPlayer in game.WaitingPlayers)
            {
                GamePlayer gamePlayer = new GamePlayer
                {
                    PlayerId = chessPlayer.Id,
                    Nickname = chessPlayer.Name,
                    Time = chessPlayer.RemainingTime,
                    Color = chessPlayer.Color,
                };
                players.Add(gamePlayer);
            }
            
            data.WaitingPlayers = players;
        }

        foreach (var gamePlayer in game.Players)
        {
            GamePlayer gamePlayerData = new GamePlayer
            {
                PlayerId = gamePlayer.Id,
                Nickname = gamePlayer.Name,
                Time = gamePlayer.RemainingTime,
                Color = gamePlayer.Color
            };
            data.Players.Add(gamePlayerData);
        }
        
        List<List<GameBoard?>> gameBoards = new();

        for (int i = 0; i < game.Board.GetLength(0); i++)
        {
            if (gameBoards.Count <= i)
                gameBoards.Add(new List<GameBoard?>());
            
            for (int j = 0; j < game.Board.GetLength(1); j++)
            {
                if (game.Board[i, j] == null)
                {
                    gameBoards[i].Add(null);
                }
                else
                {
                    var dataChessPiece = game.Board[i, j];
                    var color = game.Players.Find(p => p.Id == dataChessPiece.OwnerId).Color;
                    
                    GameBoard gameBoard = new GameBoard
                    {
                        Color = color,
                        Type = dataChessPiece.Type,
                        PieceId = dataChessPiece.ChessPieceId,
                        PersonId = dataChessPiece.OwnerId
                    };

                    gameBoards[i].Add(gameBoard);
                }
            }
        }
        data.Board = gameBoards;

        return data;
    }

    /// <summary>
    /// Выдает разрешение на вступление в игру
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока (кому выдается разрешение)</param>
    /// <param name="sendPersonId">Кто выдает разрешение</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<bool> ApprovePersonTheGame(string gameId, string playerId, string sendPersonId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        return await game.ApprovePlayer(sendPersonId, playerId);
    }
    
    /// <summary>
    /// Запрещает пользователю в игре
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="playerId">Идентификатор игрока</param>
    /// <param name="sendPersonId">Кто запрещает в игре</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<bool> RejectPersonTheGame(string gameId, string playerId, string sendPersonId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");
        
        return await game.RejectPlayer(sendPersonId, playerId);
    }

    /// <summary>
    /// Перемещает фигуры
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="personId">Идентификатор игрока</param>
    /// <param name="oldRow">Текущий ряд фигуры</param>
    /// <param name="oldCol">Текущая строка фигуры</param>
    /// <param name="newRow">Новый ряд фигуры</param>
    /// <param name="newCol">Новая строка фигуры</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task<bool> Moving(string gameId, string personId, int oldRow, int oldCol, int newRow, int newCol)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");

        return await game.MakeAMove(personId, oldRow, oldCol, newRow, newCol);
    }

    /// <summary>
    /// Позволяет игроку выйти из игры
    /// </summary>
    /// <param name="gameId">Идентификатор игры</param>
    /// <param name="personId">Идентификатор игрока</param>
    /// <exception cref="KeyNotFoundException">Данная игра не найдена</exception>
    public async Task LeaveTheGame(string gameId, string personId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new KeyNotFoundException("Данная игра не найдена");

        await game.ExitTheGame(personId);
    }

    /// <summary>
    /// Обозначает пользователя как неактивным
    /// </summary>
    /// <param name="playerId">Идентификатор игрока</param>
    public void SetUserIsInActive(string playerId)
    {
        foreach (var game in GetAllGames)
        {
            var person = game.Players.FirstOrDefault(p => p.Id == playerId);
            
            if (person != null)
                game.InActivePlayer(playerId);
        }
    }
    
    /// <summary>
    /// Обозначает пользователя как активным
    /// </summary>
    /// <param name="playerId">Идентификатор игрока</param>
    public void SetUserIsActive(string playerId)
    {
        foreach (var game in GetAllGames)
        {
            var person = game.Players.FirstOrDefault(p => p.Id == playerId);
            
            if (person != null)
                game.SetPlayerActive(playerId);
        }
    }
    
    public BaseChessGame GetChessGame(string gameId)
    {
        var game = GetAllGames.Find(x => x.GameId == gameId);

        if (game == null)
            throw new NullReferenceException("Данная игра не найдена");

        return game;
    }

    public ChessPiece? GetChessPiece(BaseChessGame game, int row, int column)
    {
        ChessPiece? piece = game.Board[row, column];
        return piece;
    }

    public ChessPlayer GetChessPlayer(BaseChessGame game, string playerId)
    {
        ChessPlayer? player = game.Players.Find(x => x.Id == playerId);
        
        if (player == null)
            throw new NullReferenceException("Данный игрок не найден");

        return player;
    }
    
    /// <summary>
    /// Возвращает все публичные игры
    /// </summary>
    public List<PublicGame>? GetAllPublicGames()
    {
        var games = GetAllGames.Where(g => g.IsGamePrivate == false && g.State == GameState.WaitingForPlayers).ToList();

        if (games.Count == 0)
            return null;
        
        List<PublicGame> publicGames = new();

        foreach (var game in games)
        {
            PublicGame publicGame = new PublicGame
            {
                Title = game.GameName,
                GameId = game.GameId,
                PlayerCount = game.Players.Count,
                TotalPlayers = game.RequiredPlayers(),
                GameMode = game.Mode.GameMode,
                IsPotion = game.IsPotion
            };
            publicGames.Add(publicGame);
        }

        return publicGames;
    }

    /// <summary>
    /// Возвращает одну рандомную публичную игру
    /// </summary>
    /// <returns>Идентификатор игры</returns>
    public string? GetRandomGames()
    {
        var games = GetAllGames.Where(g => g.IsGamePrivate == false && g.State == GameState.WaitingForPlayers).ToList();
        
        if (games.Count == 0)
            return null;

        var random = new Random();
        var randomGame = games[random.Next(games.Count)];

        return randomGame.GameId;
    }
}