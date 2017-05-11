
//Char array of 5 chars including the null character to hold the percentage string
char percentageString[6] = {'0','.','0','0','0','\0'};

//Char array of 10 chars that represent the control word
char controlWord[10] = {'D','D','D','D','D','D','D','D','D','D'};

//Buffer of 18 bytes received from the serial buffer and a termination character
char byteBuffer[19];

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

//Current range for all motors (mA)
int currentRange = 90;

//Lock to control serial reading only after a cycle has been completed
bool lock = false;

void setup() {
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

  //Program initial lowest current 
  setPotentiometerVoltage(0.28);
/*
  byteBuffer[0] = 'S';
  byteBuffer[1] = '1';
  byteBuffer[2] = '.';
  byteBuffer[3] = '0';
  byteBuffer[4] = '0';
  byteBuffer[5] = '0';
  byteBuffer[6] = ':';
  byteBuffer[7] = 'U';
  byteBuffer[8] = 'U';
  byteBuffer[9] = 'U';
  byteBuffer[10] = 'U';
  byteBuffer[11] = 'U';
  byteBuffer[12] = 'U';
  byteBuffer[13] = 'U';
  byteBuffer[14] = 'U';
  byteBuffer[15] = 'U';
  byteBuffer[16] = 'U';
  byteBuffer[17] = 'E';
  parseSerialMessage();
  */

}

void loop() {
  // put your main code here, to run repeatedly:
  
  
}

//Arduino native function called when serial data is available
void serialEvent(){
  if(!lock){
    //Wait for 18 characters to be available
    while(Serial.available() < 18) ;

    //Lock the microcontroller
    lock = true;
    
    //Read 18 characters into a buffer 
    for(char i = 0; i < 18; i ++)
      byteBuffer[i] = Serial.read();
    
    //Clear the rest of the buffer that holds other 46 unknown characters
    for(char i = 0; i < 46; i++)
      Serial.read();
  
    //Parse the received data
    parseSerialMessage();
  }
}

//Checks the received buffer for formating
void parseSerialMessage(){
  //Check for the correct format of the message
  bool format = byteBuffer[0] == 'S' && byteBuffer[17] == 'E' && byteBuffer[2] == '.' && byteBuffer[6] == ':';
  if(format){
    Serial.println("ACK");
    
    //Get percentage string
    formatPercentageString();
    
    //Get control word
    formatControlWord();

    //Manage glove changes
    controlGlove();
  }
  else
    Serial.println("NACK");

  //Release the lock
  lock = false;
  
}

//Get the percentage string from the serial message
void formatPercentageString(){
  for(char i = 1; i < 6; i++){
     percentageString[i-1] = byteBuffer[i];
  }
}

//Get the control word from the serial message
void formatControlWord(){
  for(char i = 7; i < 17; i++){
     controlWord[i-7] = byteBuffer[i];
  }
}

//Manage glove changes
void controlGlove(){

  //After parsing the serial message:

  //Set potentiometer to the required voltage
  setDigitalPotentiometer();

  //Wait some time to DC supply to stabilize
  delay(50);

  //Activate corresponding motors
  activateMotors();
}

//Set potentiometer
void setDigitalPotentiometer(){
    
    float percentage = atof(percentageString);
    int n = getNumberOfActiveMotors();
    double VpRead = analogRead(Vpot);
    double Vp = (5.0*VpRead)/1023.0;
    
    //float Is = (percentage*currentRange+110.0)*n/(10);
    double Is = (percentage*currentRange+110.0);
    double objectiveVoltage = (n*(Is - b))/(10*m) ;

    if(objectiveVoltage < 0.28 || n == 0)
      objectiveVoltage = 0.28;
    else if(objectiveVoltage > 5.2)
      objectiveVoltage = 5.2;
    setPotentiometerVoltage(objectiveVoltage);  
}


//Sets potentiometer voltage
void setPotentiometerVoltage(double objectiveVoltage){

  /**
     * Digital potentiometer counter behaviour
     * UD = 0 -> Vp increases
     * UD = 1 -> Vp decreases
     */
     double VpRead = analogRead(Vpot);
     double Vp = 5.0*VpRead/1023.0;
     double newVp;
     double difference;
     char i = 0;
     //Serial.println("Vo: " + String(objectiveVoltage,DEC));
     if(objectiveVoltage > Vp){
      while(objectiveVoltage > Vp){
        //Serial.println("Vp increasing: " + String(Vp,DEC));
        increaseVp();
        VpRead = analogRead(Vpot);
        newVp = 5.0*VpRead/1023.0;
        difference = objectiveVoltage-newVp;
        if(abs(difference) < 0.2 || i > 100)
          break;
        Vp = newVp;
        i++;
      }
      //decreaseVp();
     }
     else if(objectiveVoltage < Vp){
      while(objectiveVoltage < Vp){
        //Serial.println("Vp decreasing: " + String(Vp,DEC));
        decreaseVp();
        VpRead = analogRead(Vpot);
        newVp = 5.0*VpRead/1023.0;
        difference = objectiveVoltage-newVp;
        if(abs(difference) < 0.2 || i > 100)
          break;
        Vp = newVp;
        i++;
      }
      //increaseVp();
     }
}

//Get the number of active motors to be activated
int getNumberOfActiveMotors( ){
  int n = 0;
  for(char i = 0; i < 10; i++){
    char motorControl = controlWord[i];
    if(motorControl == 'U')
      n++;
  }
  return n;
}

//Activate motors with the control word
void activateMotors(){
  float percentage = atof(percentageString);
  if(percentage < 0.05)
    deactivateAllMotors();
  else{
    for(char i = 0; i < 10; i++){
      char motorControl = controlWord[i];
      if(motorControl == 'U')
        digitalWrite(i+2, HIGH);
      else
        digitalWrite(i+2, LOW);
    }
  }
}

//Deactivate all motors
void deactivateAllMotors(){
  for(char i = 0; i < 10; i++){
      digitalWrite(i+2, LOW);
  } 
}

//Move potentiometer wiper
void movePotCounter(){
  digitalWrite(INC, LOW);
  delay(1);
  digitalWrite(INC, HIGH);
}

//Increase potentiometer voltage
void increaseVp(){
  if(counterDirection){
    counterDirection = false;
    digitalWrite(UD, LOW);
  }
  delay(1);
  movePotCounter();
  delay(5);
}

//Decrease potentiometer voltage
void decreaseVp(){
  if(!counterDirection){
    counterDirection = true;
    digitalWrite(UD, HIGH);
  }
  delay(1);
  movePotCounter();
  delay(5);
}




