#include <iostream>

using namespace std;


//	Define global program variables
// FIX: Hardcoded, non-dynamic data
string name1 = "Bob Jones";
int num1 = 1;

string name2 = "Sarah Davis";
int num2 = 2;

string name3 = "Amy Friendly";
int num3 = 1;

string name4 = "Johnny Smith";
int num4 = 1;

string name5 = "Carol Spears";
int num5 = 2;

// FIX: Unsecure password, not encrypted, stored statically
string password = "123";


/// <summary>
/// Prompts the user to enter a username and password. Validates the entered input string.
/// </summary>
/// <returns>
/// 1 = authentication sucessful.
/// 2 = authentication failed.
/// </returns>
int CheckUserPermissionAccess()
{

	string user, pass;

	cout << "Enter your username: \n";
	// FIX: No input validation
	cin >> user;

	cout << "Enter your password: \n";
	// FIX: No input validation
	cin >> pass;

	if (password.compare(pass) == 0)
	{
		return 1;
	}
	else {

		return 2;

	}

}


/// <summary>
/// A big print function that formats and dumps the program variables cleanly.
/// </summary>
void DisplayInfo()
{

	cout << "  Client's Name    Service Selected (1 = Brokerage, 2 = Retirement)" << endl;

	cout << "1. " << name1 << " selected option " << num1 << endl;
	cout << "2. " << name2 << " selected option " << num2 << endl;
	cout << "3. " << name3 << " selected option " << num3 << endl;
	cout << "4. " << name4 << " selected option " << num4 << endl;
	cout << "5. " << name5 << " selected option " << num5 << endl;

	return;

}


/// <summary>
/// Changes the choice value with an associated customer
/// </summary>
void ChangeCustomerChoice()
{

	cout << "Enter the number of the client that you wish to change\n";
	int changechoice;
	// FIX: No input validation
	cin >> changechoice;

	cout << "Please enter the client's new service choice (1 = Brokerage, 2 = Retirement)\n";
	int newservice;
	// FIX: No input validation
	cin >> newservice;

	// FIX: static structure shoulc be dynamic
	if (changechoice == 1)
	{

		num1 = newservice;

	}
	else if (changechoice == 2)
	{

		num2 = newservice;

	}
	else if (changechoice == 3)
	{

		num3 = newservice;

	}
	else if (changechoice == 4)
	{

		num4 = newservice;

	}
	else if (changechoice == 5)
	{

		num5 = newservice;

	}

}


/// <summary>
/// Main Program. After printing the introduction messages it authenticates the user and starts a menu loop.
/// </summary>
/// <returns>0</returns>
int main()
{

	//	Added per the instructions
	cout << "Created by A. Williams" << endl;


	//	Startup message
	cout << "Hello! Welcome to our Investment Company\n";


	//	Authentication
	int answer;
	do
	{
		// FIXED: 'Workaround'.. This function can crash with bad user input
		try {
			answer = CheckUserPermissionAccess();
		}
		catch (...) {

			cout << "Invalid Password. Please try again\n";
		
		}

		if (answer != 1)
		{
			cout << "Invalid Password. Please try again\n";
		}

	} while (answer != 1);


	//	Main Menu
	int choice;
	do
	{

		cout << "What would you like to do?\n";
		cout << "DISPLAY the client list (enter 1)\n";
		cout << "CHANGE a client's choice (enter 2)\n";
		cout << "Exit the program.. (enter 3)\n";


		//	FIXED: secure input validation for a menu choice
		while (!(cin >> choice) || choice < 1 || choice > 3) {

			cout << "Invalid input. Please enter a number between 1 and 3: " << endl;
			cin.clear();
			cin.ignore(numeric_limits<streamsize>::max(), '\n');

		}

		cout << "You chose " << choice << endl;

		if (choice == 1)
		{

			DisplayInfo();

		}
		else if (choice == 2)
		{

			ChangeCustomerChoice();

		}

	} while (choice != 3);


	//	Return 0
	return 0;

}
