﻿namespace AggregatorMicroservice.Models.DTOs.Incoming;

public class SignUpIncomingDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ReEnteredPassword { get; set; }
    public string Role { get; set; }
}
