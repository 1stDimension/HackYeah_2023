﻿using System.ComponentModel.DataAnnotations;

namespace HackYeah.Backend.Keystore.Data;

public class Config
{
    [Required]
    public string DatabaseFile { get; set; }
}
