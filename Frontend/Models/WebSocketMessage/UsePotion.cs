﻿using Frontend.Enums;

namespace Frontend.Models.WebSocketMessage;

public class UsePotion: BaseWebSocketMessage
{
    public PotionType PotionType { get; set; }
    public required string PotionName { get; set; }
    
    public required string UsePersonId { get; set; }
    public required string UsePersonName { get; set; }
    
    
}