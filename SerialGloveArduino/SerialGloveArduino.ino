#include <SoftwareSerial.h>

//Message
String incomingMessage = "";

//Maximum current (mA)
int maxIs = 183;

//U/D' control of digital potentiometer
int UD = 12;

//INC control of digital potentiometer
int INC = 13;

//Linear regression variables for supply control( Is = m*Vpot + b
float m = 32.555248527564;
float b = 19.580882300717;

//ADC input for digital potentiometer voltage
int Vpot = A0;

//Current UD value
char counterDirection;

void setup() {
  // put your setup code here, to run once:

  //Serial initialization
  Serial.begin(9600);

  //Motor signals initialization
  for(char i = 2; i <= 11; i++){
    pinMode(i,OUTPUT);
    digitalWrite(i,LOW);
  }

  //Digital potentiometer initialization
  pinMode(UD,OUTPUT);
  pinMode(INC, OUTPUT);
  counterDirection = LOW;
  digitalWrite(UD, counterDirection);
  digitalWrite(INC, HIGH);
}

void loop() {
  // put your main code here, to run repeatedly:
  readSerialLine();
  gloveHandler();
  incomingMessage = "";
} 

void readSerialLine(){
  if(Serial.available() >0){
    while(Serial.available() < 16) ;
    char byteReceived = Serial.read();
    //Look for starting character
    char i = 0;
    while(byteReceived != 'S' && i < 16){
      byteReceived = Serial.read();
      i ++;
    }
    //Starting character not found, message incomplete
    if(i == 15)
      return;
  
    //Start storing message
    byteReceived = Serial.read();
    while(byteReceived != '\n'){
      incomingMessage.concat(byteReceived);
      byteReceived = Serial.read();
    }
  }
}


void gloveHandler(){
  if(incomingMessage != ""){
    String percentage = getValue(incomingMessage, ':', 0);
    String controlWord = getValue(incomingMessage, ':', 1);
    //Set digital potentiometer for supply
    setDigitalPotentiometer(percentage);
    //Wait some time to DC supply to stablelize
    delay(50);
    //Activate motors
    activateMotors(controlWord);
  }
}

void setDigitalPotentiometer(String percentageS){
    float percentage = percentageS.toFloat();
    float Is = percentage*maxIs;
    float objectiveVoltage = (Is - b)/m;

    /**
     * Digital potentiometer counter behaviour
     * UD = 0 -> Vp increases
     * UD = 1 -> Vp decreases
     */
     float Vp = 5.0*analogRead(Vpot)/1023.0;
     float newVp;
     float difference;
     if(objectiveVoltage > Vp){
      while(objectiveVoltage > Vp){
        increaseVp();
        newVp = 5.0*analogRead(Vpot)/1023.0;
        difference = Vp-newVp;
        if(abs(difference) < 0.05)
          return;
        Vp = newVp;
      }
      decreaseVp();
     }
     else if(objectiveVoltage < Vp){
      while(objectiveVoltage < Vp){
        decreaseVp();
        newVp = 5.0*analogRead(Vpot)/1023.0;
        difference = Vp-newVp;
        if(abs(difference) < 0.05)
          return;
        Vp = newVp;
      }
      increaseVp();
     }
     
}

void activateMotors(String controlWord){
  for(char i = 0; i < 10; i++){
    char motorControl = controlWord.charAt(i);
    if(motorControl == '1')
      digitalWrite(i+2, HIGH);
    else if(motorControl == '0')
      digitalWrite(i+2, LOW);
  }
}


void movePotCounter(){
  digitalWrite(INC, LOW);
  delay(1);
  digitalWrite(INC, HIGH);
}

void increaseVp(){
  if(counterDirection != LOW)
    counterDirection = LOW;
  digitalWrite(UD, counterDirection);
  delay(1);
  movePotCounter();
}

void decreaseVp(){
  if(counterDirection != HIGH)
    counterDirection = HIGH;
  digitalWrite(UD, counterDirection);
  delay(1);
  movePotCounter();
}

String getValue(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length()-1;

  for(int i=0; i<=maxIndex && found<=index; i++){
    if(data.charAt(i)==separator || i==maxIndex){
        found++;
        strIndex[0] = strIndex[1]+1;
        strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }

  return found>index ? data.substring(strIndex[0], strIndex[1]) : "";
}

