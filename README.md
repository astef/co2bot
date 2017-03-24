# co2bot

Telegram bot (russian) which notifies about air quality metrics (CO2 &amp; temperature)

Integrated with CSV files from this device: <https://dadget.ru/detektor-co2>

Sends notifications via GET request to URL.

## Options

*Option* | *Description*
--- | ---
Frequency | How othen should we check input file for new values
LongRepeatDelayMin | Cooldown between repeating alarm if problem still exists. Reset if params returns to normal range. Starts after all *ShortRepeatCount* exhausted.
ShortRepeatCount | Repeat alarm 
Co2DataPath | Path to zg program output dir
RedCo2 | Lower bound of critical CO2 range
YellowCo2 | Lower bound of warning CO2 range
RedTemp | Upper bound of normal temperature range in Celsuis
BlueTemp | Lower bound of normal temperature range in Celsuis
HttpUrl | HTTP URL where GET request should be send. Should be set with parameters: ```http://example.org?text="```. Useful text will be appended to the end of URL to form a proper request.
MorningTime | End of silent mode time
EveningTime | Start of silent mode time


### Example

```javascript
{
  "Frequency": 60000, // in ms
  "LongRepeatDelayMin": 60, // in minutes
  "ShortRepeatCount": 3,
  "Co2DataPath": "<your path>", // path to ZG View
  "SendToConsole": false, // send to console, not HTTP (for debug)
  "RedCo2": 1200,
  "YellowCo2": 800,
  "RedTemp": 24,
  "BlueTemp": 22,
  "HttpUrl": "<YOUR URL HERE>",
  "MorningTime": "07:00", // working period
  "EveningTime": "23:00"
}
```
