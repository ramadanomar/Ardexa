#include <TimeLib.h>
#include <Time.h>
#include "sunMoon.h"
#include <dht.h>

dht DHT;

#define DHT11_PIN 13 // Pentru a apela pin-ul printr-un nume nu valoare

#define TIME_HEADER "T"
#define TIME_REQUEST 7 // ASCII BELL CHARACTER REQ A Time Sync Message

#define OUR_latitude    26.1025              // Bucharest cordinates
#define OUR_longtitude  44.4268
#define OUR_timezone    120


sunMoon sm;

const int buzzer = 9; //buzzer to arduino pin 9
const int buttonPin = 3; // button Pin = 3
int buttonState = 0; // State of the button

char incomingdata;  
void setup() {  
    pinMode(6, OUTPUT);  
    Serial.begin(9600);
    setSyncProvider( requestSync ); // set function to call when sync required
    Serial.println("Waiting for sync message");
    pinMode(buzzer, OUTPUT); // Set buzzer - pin 9 as an output
    pinMode(buttonPin, INPUT); // Set Button - pin T as INPUT
}  
void loop() {  
        if ( Serial.available() ) { // Waiting for a response
        processSyncMessage();
        }
       // whatPhase();
    incomingdata = Serial.read(); {  

        switch( incomingdata ) {
          case 'c' : // Alarma - Start cronometru - Cand primeste cuvantul "ALARMA"
            PrimaAlarma();
            break;
          case 'm': // Pauza de Masa - cand aude / primeste de la buton cuvantul pauza de masa
            cronometru();
            break;
          case 's': // Fazele soarelui
            whatPhase();
            break;
          case 'u': // Umiditate si Temperatura
            afisareTemperatura();
            break;
        }
    }  
}  

void digitalClockDisplay() { // Afisare Timp
  Serial.print(hour());
  printDigits(minute());
  printDigits(second());
  Serial.print(" ");
  Serial.print(day());
  Serial.print(" ");
  Serial.print(month());
  Serial.print(" ");
  Serial.print(year());
  Serial.println();
}

void printDigits(int digits) { // Punem un 0 in fata daca este mai mic de 10 ex 13:07 in loc de 13:7 pentru 13 ore si 7 minute.
  Serial.print(":");
  if(digits < 10);
    Serial.print('0');
  Serial.print(digits);
}

void processSyncMessage() {
  unsigned long pctime; // timp actual
  const unsigned long DEFAULT_TIME = 1357041600; // Jan 1 2013

  if(Serial.find(TIME_HEADER)) {
    pctime = Serial.parseInt();
    if( pctime >= DEFAULT_TIME) { // check the integer is a valid time ( Greater than Jan 1 2013 )
      setTime(pctime);
    }
  }
}

void cronometru()
{
  delay(60 * 30) // delay 30s
  Alarma();
}

void Alarma()
{
  tone(buzzer, 1000); // Send 1KHz sound signal...
  delay(120);        // ...pt 2 minute
  noTone(buzzer);     // Stop sound...
}

void PrimaAlarma()
{
  Alarma();
  cronometru();
}

void sunPhases()
{
  sm.init(OUR_timezone, OUR_latitude, OUR_longtitude);
  uint32_t jDay = sm.julianDay();               // Optional call
  byte mDay = sm.moonDay();
  time_t sRise = sm.sunRise();
  time_t sSet  = sm.sunSet();
}

time_t requestSync()
{
  Serial.write(TIME_REQUEST);
  return 0;
}

void whatPhase()
{
  time_t sRise = sm.sunRise();
  time_t sSet  = sm.sunSet();
  Serial.print( sRise );
  Serial.println(" is the Sunrise ");
  Serial.print( sSet );
  Serial.println(" is the Sunset ");
}

void afisareTemperatura() // Afisare Temperatura
{
  int chk = DHT.read11(DHT11_PIN);
  Serial.print("Temperatura = ");
  Serial.println(DHT.temperature);
  Serial.print("Humiditate = ");
  Serial.println(DHT.humidity);
}
