# co2bot
Telegram bot (russian) which notifies about air quality metrics (CO2 &amp; temperature)

Integrated with CSV files from this device: https://dadget.ru/detektor-co2

Sends notifications via GET request to URL.

Options:
```javascript
{
  "Frequency": 60000, // in ms
  "LongRepeatDelayMin": 60, // in minutes
  "ShortRepeatCount": 3,
  "Co2DataPath": "", // path to ZG View
  "SendToConsole": false, // send to console, not HTTP (for debug)
  "RedCo2": 1200,
  "YellowCo2": 800,
  "RedTemp": 24,
  "BlueTemp": 22,
  "HttpUrl": "https://adapter.yamstr.com/0bddbaf7369d86a1a674234557b3872c/-168138256?text=",
  "MorningTime": "07:00", // working period
  "EveningTime": "23:00"
}
```
