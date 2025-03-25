namespace WeatherApi.Model
{
    public record Coord(double Lon, double Lat);

    public record Weather(int Id, string Main, string Description, string Icon);

    public record Main(
        double Temp,
        double FeelsLike,
        double TempMin,
        double TempMax,
        int Pressure,
        int Humidity,
        int SeaLevel,
        int GrndLevel
    );

    public record Wind(double Speed, int Deg);

    public record Clouds(int All);

    public record Sys(
        int Type,
        int Id,
        string Country,
        long Sunrise,
        long Sunset
    );

    public record WeatherPayload(
        Coord Coord,
        List<Weather> Weather,
        string Base,
        Main Main,
        int Visibility,
        Wind Wind,
        Clouds Clouds,
        long Dt,
        Sys Sys,
        int Timezone,
        int Id,
        string Name,
        int Cod
    );
}

