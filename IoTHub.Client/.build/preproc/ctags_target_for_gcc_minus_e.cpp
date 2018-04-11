# 1 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
# 1 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
# 2 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 3 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 4 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 5 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 6 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 7 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 8 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 9 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2
# 10 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 2

enum STATUS
{
 Idle,
 SendingToAzure
};

int wifiStatus = WL_DISCONNECTED;
static STATUS status = Idle;
static bool has_main_WiFi = false;
static bool has_emergency_WiFi = false;
int local_port = 2390;
int message_count = 1;
static bool message_sending = true;
static uint64_t send_interval_ms;
static uint64_t send_emergency_interval_ms;
WiFiUDP Udp;
int udp_data_count = 0;
char packetBuffer[256];
char timestamp[80];

char *Global_Device_Endpoint = "global.azure-devices-provisioning.net";
char *ID_Scope = "0ne00013349";
char *registrationId = "";
//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Utilities

void EnterIdleState()
{
 ScreenSetup();
 status = Idle;
}

void ScreenSetup()
{
 Screen.print(0, "DevKit");
 Screen.print(1, " ");
 Screen.print(2, " ");
 has_main_WiFi ? Screen.print(3, "> WiFi on") : Screen.print(3, "> WiFi off");
}

void InitWiFi()
{
 has_main_WiFi = false;
 Screen.print(1, "Connecting...");
 wifiStatus = WiFi.begin();
 if (wifiStatus != WL_CONNECTED)
 {
  has_main_WiFi = false;
  ScreenSetup();
  has_emergency_WiFi = EmergencyConnection();
  if (has_emergency_WiFi)
   EmergencyConnectLED();
  EnterIdleState();
  return;
 }
 Udp.begin(local_port);
 WiFiConnectLED();
 SetupAPServer();
 has_main_WiFi = true;
 EnterIdleState();
}

void SendForwardEmergencyMessage(const char *messageToSendForward)
{
 if (has_main_WiFi)
 {
  if ((int)(SystemTickCounterRead() - send_emergency_interval_ms) >= getInterval())
  {
   EVENT_INSTANCE *message = DevKitMQTTClient_Event_Generate(messageToSendForward, MESSAGE);
   DevKitMQTTClient_SendEventInstance(message);
   send_emergency_interval_ms = SystemTickCounterRead();
  }
  else
  {
   DevKitMQTTClient_Check();
  }
 }
 EnterIdleState();
}

void ReceiveFromClient()
{
 if (has_main_WiFi)
 {
  Screen.print(1, "Listening...");
  char packet[256];
  int packetSize = Udp.read(packet, 256);
  if (packetSize > 0)
  {
   SendForwardEmergencyMessage(packet);
   Screen.print(2, "Msg arrived");
   if (Udp.beginPacket(Udp.remoteIP(), Udp.remotePort()))
   {
    GetTime(timestamp);
    Udp.write((const uint8_t *)timestamp, sizeof(timestamp));
    Udp.endPacket();
   }
   EmergencyMessageLED();
  }
 }
 EnterIdleState();
}

void SendData()
{
 if (has_emergency_WiFi)
 {
  if (Udp.beginPacket(WiFi.gatewayIP(), local_port) == 1)
  {
   Screen.print(1, "Emrg sending...");
   Udp.write((const uint8_t *)packetBuffer, sizeof(packetBuffer));
   Udp.endPacket();
   Udp.read(timestamp, 80);
   EmergencyMessageLED();
   udp_data_count++;
  }
 }
 if (udp_data_count > 100)
 {
  InitWiFi();
  udp_data_count = 0;
  return;
 }
 EnterIdleState();
}

void SendToAzure()
{
 if (!has_emergency_WiFi && !has_main_WiFi)
  InitWiFi();
 if (message_sending && (int)(SystemTickCounterRead() - send_interval_ms) >= getInterval())
 {
  Screen.print(1, "Sending...");
  if (has_main_WiFi)
   GetTime(timestamp);
  MessageToAzure(message_count++, packetBuffer, 256, timestamp);
  if (has_emergency_WiFi)
  {
   SendData();
   send_interval_ms = SystemTickCounterRead();
   return;
  }
  EVENT_INSTANCE *message = DevKitMQTTClient_Event_Generate(packetBuffer, MESSAGE);
  DevKitMQTTClient_SendEventInstance(message);
  send_interval_ms = SystemTickCounterRead();
 }
 else
 {
  if (has_emergency_WiFi)
   return;
  DevKitMQTTClient_Check();
 }
 ScreenSetup();
 ReceiveFromClient();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Callbacks
static void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result)
{
 if (result == IOTHUB_CLIENT_CONFIRMATION_OK)
 {
  AzureConfirmationLED();
 }
}

static void messageCallback(const char *payLoad, int size)
{
 char buff[256];
 snprintf(buff, 256, "%s", payLoad);
 Screen.print(1, "Msg arrived:");
 Screen.print(2, buff);
 EmergencyMessageLED();
}

int deviceMethodCallback(const char *methodName, const unsigned char *payload, int size, unsigned char **response, int *response_size)
{
 do{{ LOGGER_LOG l = xlogging_get_log_function(); if (l != 
# 188 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 3 4
__null
# 188 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
) l(AZ_LOG_INFO, (strrchr("C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino", '/') ? strrchr("C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino", '/') + 1 : "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"), __func__, 188, 0x01, "Try to invoke method %s", methodName); }; }while((void)0,0);
 const char *responseMessage = "\"Successfully invoke device method\"";
 int result = 200;

 if (strcmp(methodName, "start") == 0)
 {
  message_sending = true;
 }
 else if (strcmp(methodName, "stop") == 0)
 {
  message_sending = false;
 }
 else
 {
  do{{ LOGGER_LOG l = xlogging_get_log_function(); if (l != 
# 202 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 3 4
 __null
# 202 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
 ) l(AZ_LOG_INFO, (strrchr("C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino", '/') ? strrchr("C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino", '/') + 1 : "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"), __func__, 202, 0x01, "No method %s found", methodName); }; }while((void)0,0);
  responseMessage = "\"No method found\"";
  result = 404;
 }

 *response_size = strlen(responseMessage);
 *response = (unsigned char *)malloc(*response_size);
 strncpy((char *)(*response), responseMessage, *response_size);

 return result;
}

static void deviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size)
{
 char *temp = (char *)malloc(size + 1);
 if (temp == 
# 217 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino" 3 4
            __null
# 217 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
                )
 {
  return;
 }
 memcpy(temp, payLoad, size);
 temp[size] = '\0';
 parseTwinMessage(updateState, temp);
 free(temp);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch
void setup()
{
 Screen.init();
 ScreenSetup();
 SetupDeviceId();
 pinMode(USER_BUTTON_B, 0x1);
 pinMode(USER_BUTTON_A, 0x1);
 Serial.begin(115200);
 InitWiFi();
 sensorInit();

 DevkitDPSClientStart(Global_Device_Endpoint, ID_Scope, registrationId);
 DevKitMQTTClient_Init(true);
 DevKitMQTTClient_SetSendConfirmationCallback(SendConfirmationCallback);
 DevKitMQTTClient_SetMessageCallback(messageCallback);
 DevKitMQTTClient_SetDeviceTwinCallback(deviceTwinCallback);
 DevKitMQTTClient_SetDeviceMethodCallback(deviceMethodCallback);
}

void DoIdle()
{
 if (digitalRead(USER_BUTTON_B) == 0x1 && digitalRead(USER_BUTTON_A) == 0x1)
 {
  status = SendingToAzure;
  return;
 }
}

void loop()
{
 switch (status)
 {
 case Idle:
  DoIdle();
  break;

 case SendingToAzure:
  SendToAzure();
  break;
 }
 delay(10);
}
