#include <Arduino.h>
#line 1 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
#line 1 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
#include "AZ3166WiFi.h"
#include "AZ3166WiFiUdp.h"
#include "OLEDDisplay.h"
#include "AzureIotHub.h"
#include "DevKitMQTTClient.h"
#include "SystemTickCounter.h"
#include "utility.h"
#include "config.h"
#include "DevkitDPSClient.h"

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
char packetBuffer[MESSAGE_MAX_LEN];
char timestamp[80];

char *Global_Device_Endpoint = "global.azure-devices-provisioning.net";
char *ID_Scope = "0ne00013349";
char *registrationId = "";
//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Utilities

#line 37 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void EnterIdleState();
#line 43 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void ScreenSetup();
#line 51 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void InitWiFi();
#line 73 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void SendForwardEmergencyMessage(const char *messageToSendForward);
#line 91 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void ReceiveFromClient();
#line 114 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void SendData();
#line 137 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void SendToAzure();
#line 169 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
static void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result);
#line 177 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
static void messageCallback(const char *payLoad, int size);
#line 186 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
int deviceMethodCallback(const char *methodName, const unsigned char *payload, int size, unsigned char **response, int *response_size);
#line 214 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
static void deviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size);
#line 229 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void setup();
#line 248 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void DoIdle();
#line 257 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
void loop();
#line 37 "C:\\Users\\piter\\source\\repos\\Novlex\\IoTHub.Client\\NovlexTeam.ino"
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
		char packet[MESSAGE_MAX_LEN];
		int packetSize = Udp.read(packet, MESSAGE_MAX_LEN);
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
		MessageToAzure(message_count++, packetBuffer, MESSAGE_MAX_LEN, timestamp);
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
	char buff[MESSAGE_MAX_LEN];
	snprintf(buff, MESSAGE_MAX_LEN, "%s", payLoad);
	Screen.print(1, "Msg arrived:");
	Screen.print(2, buff);
	EmergencyMessageLED();
}

int deviceMethodCallback(const char *methodName, const unsigned char *payload, int size, unsigned char **response, int *response_size)
{
	LogInfo("Try to invoke method %s", methodName);
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
		LogInfo("No method %s found", methodName);
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
	if (temp == NULL)
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
	pinMode(USER_BUTTON_B, INPUT);
	pinMode(USER_BUTTON_A, INPUT);
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
	if (digitalRead(USER_BUTTON_B) == HIGH && digitalRead(USER_BUTTON_A) == HIGH)
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
	delay(LOOP_DELAY);
}
