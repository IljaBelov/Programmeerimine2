#include <iostream>
#include <ctime>
#include <iomanip>
#include "httplib.h"
#include "json.hpp"

using json = nlohmann::json;

struct Hours {
    int currentHour;
    int hourPlus3;
};

// Функция для получения текущего часа и +3
Hours getHours() {
    std::time_t now = std::time(nullptr);
    std::tm localTime;
    localtime_s(&localTime, &now);

    Hours result;
    result.currentHour = localTime.tm_hour;
    result.hourPlus3 = (localTime.tm_hour + 3) % 24;

    return result;
}

// Функция для получения JSON от API и поиска температуры по конкретному часу
double getTemperatureForHour(int hour) {
    httplib::SSLClient cli("api.met.no"); // HTTPS
    cli.set_default_headers({
        {"User-Agent", "MyWeatherApp/1.0"}
        });

    auto res = cli.Get("/weatherapi/locationforecast/2.0/compact?lat=60&lon=11");
    if (!res || res->status != 200) {
        std::cerr << "Ошибка HTTP запроса!" << std::endl;
        return -999; // ошибка
    }

    // Парсим JSON
    auto j = json::parse(res->body);

    // Ищем температуру по часу
    // Данные идут в j["properties"]["timeseries"], каждый элемент имеет ["time"] и ["data"]["instant"]["details"]["air_temperature"]
    for (auto& item : j["properties"]["timeseries"]) {
        std::string timeStr = item["time"];
        int itemHour = std::stoi(timeStr.substr(11, 2)); // HH из "2026-03-02T14:00:00Z"

        if (itemHour == hour) {
            double temp = item["data"]["instant"]["details"]["air_temperature"];
            return temp;
        }
    }

    return -999; // если час не найден
}

int main() {
    Hours h = getHours();

    std::cout << "Current hour: " << h.currentHour << std::endl;
    std::cout << "Hour + 3: " << h.hourPlus3 << std::endl;

    double tempNow = getTemperatureForHour(h.currentHour);
    double tempPlus3 = getTemperatureForHour(h.hourPlus3);

    std::cout << "Temperature now: " << tempNow << "C" << std::endl;
    std::cout << "Temperature in +3 hours: " << tempPlus3 << "C" << std::endl;
}