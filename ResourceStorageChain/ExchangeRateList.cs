﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ExchangeRateList
{
    public string Disclaimer { get; set; }
    public string License { get; set; }
    public long Timestamp { get; set; }
    public string Base { get; set; }
    public Dictionary<string, double> Rates { get; set; }
}
