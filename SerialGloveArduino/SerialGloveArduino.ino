#include <SoftwareSerial.h>

//Message
String incomingMessage = "";

//Maximum current (mA)
int maxIs = 183;

//U/D' control of digital potentiometer
int UD = 12;

//INC control of digital potentiometer
int INC = 13;

//Linear regression variables for supply control( Is = m*Vpot + b)
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
  String msg = readSerialLine();
  gloveHandler(msg);
} 

String readSerialBuffer(){
  String msg = "";
  if(Serial.available() >= 16){
    
    //Read 16 bytes
    char bytes[17];
    char i = 0;
    while(i < 17){
     bytes[i] = Serial.read();
     i++;
    }
  }
  return msg;
}

String readSerialLine(){
  String msg = "";
  if(Serial.available() >0){
    while(Serial.available() < 16) ;
    char byteReceived = Serial.read();
    
    //Starting character not found, message incomplete
    if(byteReceived != 'S')
      return "";
  
    //Start storing message
    byteReceived = Serial.read();
    char count = 0;
    while(count < 15 ){
      msg.concat(byteReceived);
      byteReceived = Serial.read();
      count++;
    }
  }
  return msg;
}


void gloveHandler(String msg){

  if(msg != incomingMessage && msg != ""){
    incomingMessage = msg;
    String controlWord = getControlWord();
    String percentage = getPercentage();
    //Set digital potentiometer for supply
    setDigitalPotentiometer(percentage, controlWord);
    //Wait some time to DC supply to stabilize
    delay(50);
    //Activate motors
    activateMotors(controlWord);
  }
}

void setDigitalPotentiometer(String percentageS, String controlWord){
    float percentage = percentageS.toFloat();
    int n = getNumberOfActiveMotors(controlWord);
    float Vp = 5.0*analogRead(Vpot)/1023.0;
    if(n==0){
      if(Vp >= 2.8)
        setDigitalPotentiometer(2.0f);
      return;
    }
    float Is = percentage*maxIs*n/(10.0f);
    //float Is = percentage*maxIs;
    float objectiveVoltage = (Is - b)/m;
    setDigitalPotentiometer(objectiveVoltage);
   
}

void setDigitalPotentiometer(float objectiveVoltage){

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

int getNumberOfActiveMotors(String controlWord){
  int n = 0;
  for(char i = 0; i < 10; i++){
    char motorControl = controlWord.charAt(i);
    if(motorControl == 'U')
      n++;
  }
  return n;
}

void activateMotors(String controlWord){
  for(char i = 0; i < 10; i++){
    char motorControl = controlWord.charAt(i);
    if(motorControl == 'U')
      digitalWrite(i+2, HIGH);
    else
      digitalWrite(i+2, LOW);
  }
}

void deactivateAllMotors(){
  for(char i = 0; i < 10; i++){
      digitalWrite(i+2, LOW);
  } 
}

void movePotCounter(){
  digitalWrite(INC, LOW);
  delay(1);
  digitalWrite(INC, HIGH);
}

void increaseVp(){
  if(counterDirection != LOW){
    counterDirection = LOW;
    digitalWrite(UD, counterDirection);
  }
  delay(1);
  movePotCounter();
  delay(5);
}

void decreaseVp(){
  if(counterDirection != HIGH){
    counterDirection = HIGH;
    digitalWrite(UD, counterDirection);
  }
  delay(1);
  movePotCounter();
  delay(5);
}

String getControlWord(){
  String controlWord = "";
  for(char i = 5; i < 15; i++)
    controlWord.concat(incomingMessage.charAt(i));
  return controlWord;
}

String getPercentage(){
  String percetnage = "";
  for(char i = 0; i < 4; i++)
    percetnage.concat(incomingMessage.charAt(i));
}


