//#include <SoftwareSerial.h>
//#include <SerialCommand.h>
//SerialCommand sCmd;

//Message
String incomingMessage = "";

//Current range for all motors (mA)
int currentRange = 73;

//U/D' control of digital potentiometer
int UD = 12;

//INC control of digital potentiometer
int INC = 13;

//Linear regression variables for supply control( Is = m*Vpot + b)
double m = 32.555248527564;
double b = 19.580882300717;

//ADC input for digital potentiometer voltage
int Vpot = A0;

//Current UD value
bool counterDirection;

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
  counterDirection = false;
  digitalWrite(UD, LOW);
  digitalWrite(INC, HIGH);

  setDigitalPotentiometer(0.28);
}

void loop() {
  // put your main code here, to run repeatedly:
  String msg = readSerialLine();
  //String msg = "0.01:UUDDDDDDDU";
  gloveHandler(msg);
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
    char count = 0;
    while(count < 15 ){
      byteReceived = Serial.read();
      msg.concat(byteReceived);
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
    delay(100);
    //Activate motors
    activateMotors(controlWord);
  }
}

void setDigitalPotentiometer(String percentageS, String controlWord){
    
    float percentage = percentageS.toFloat();
    int n = getNumberOfActiveMotors(controlWord);
    double VpRead = analogRead(Vpot);
    double Vp = (5.0*VpRead)/1023.0;

    //float Is = (percentage*currentRange+110.0)*n/(10.0f);
    double Is = (percentage*currentRange+110.0);
    double objectiveVoltage = (Is - b)/m;
    if(objectiveVoltage < 0.28)
      objectiveVoltage = 0.28;
    else if(objectiveVoltage > 5.0)
      objectiveVoltage = 5.0;
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
     //Serial.println("Initial Vp:"+ String(Vp, DEC));
     //Serial.println("Objective Vp:"+ String(objectiveVoltage, DEC));
     if(objectiveVoltage > Vp){
      while(objectiveVoltage > Vp){
        //Serial.println("Vp:"+ String(Vp, DEC));
        increaseVp();
        newVp = 5.0*analogRead(Vpot)/1023.0;
        difference = objectiveVoltage-newVp;
        if(abs(difference) < 0.1)
          break;
        Vp = newVp;
      }
      decreaseVp();
     }
     else if(objectiveVoltage < Vp){
      while(objectiveVoltage < Vp){
        //Serial.println("Vp:"+ String(Vp, DEC));
        decreaseVp();
        newVp = 5.0*analogRead(Vpot)/1023.0;
        difference = objectiveVoltage-newVp;
        
        if(abs(difference) < 0.1)
          break;
        Vp = newVp;
      }
      increaseVp();
     }
     //Serial.println("Final Vp:"+ String(Vp, DEC));
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
  if(counterDirection){
    counterDirection = false;
    digitalWrite(UD, LOW);
  }
  delay(1);
  movePotCounter();
  delay(5);
}

void decreaseVp(){
  if(!counterDirection){
    counterDirection = true;
    digitalWrite(UD, HIGH);
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
  String percentage = "";
  for(char i = 0; i < 4; i++)
    percentage.concat(incomingMessage.charAt(i));
  return percentage;
    
}


