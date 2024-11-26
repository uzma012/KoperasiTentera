# README

## Getting Started

This README provides instructions on how to set up and run the application, including database setup, configuration, and available API endpoints.

### Database Setup

1. **Run SQL Script**: Execute the provided SQL script to create the necessary tables and insert sample data into your database. This script will set up the `Products`, `Ads`, and `UserOTP` tables.

### Configuration

2. **Update Connection String**: Open the `appsettings.json` file in your project. Replace the existing connection string with your own database connection string to ensure the application can connect to your database.

### API Endpoints

The following APIs have been implemented:

- **Registration**: 
  - **Description**: Allows users to register. Upon registration, an OTP (One-Time Password) is generated and sent to both the user's phone number and email address. The OTP will be saved in the `UserOTP` table.

- **Resend OTP**: 
  - **Description**: If the OTP expires, users can request a new OTP to be sent to their registered phone number and email.

- **OTP Verification**: 
  - **Description**: Users can verify their OTP for registration or login purposes.

- **Set PIN**: 
  - **Description**: Allows users to set a security PIN for added account protection.

- **Reset PIN**: 
  - **Description**: Resets the user's PIN to null, effectively removing the security PIN.

- **Login**: 
  - **Description**: Allows users to log in. If the user exists, an OTP will be sent to their registered phone number and email address, similar to the registration process.

- **Authorise**: 
  - **Description**: If there is requirement to login user using totp then this API will be used. It returns the token that will be used to login.

- **Products**: 
  - **Description**: Retrieves a list of available products from the database.

- **Ads**: 
  - **Description**: Retrieves a list of active advertisements.

### API Documentation

- **Swagger UI**: Swagger is integrated into the application for API documentation. You can access it by navigating to `/swagger` in your browser after running the application. This interface will provide detailed information on all available endpoints, including request/response formats and examples.

### Running the Application

- **Start the Project**: Launch the `ProductsAndAds` project to initiate the API for managing products, ads, and user registration/login flows.

### Conclusion

This application provides a comprehensive API for user registration, OTP management, and retrieval of product and ad data. Follow the steps above to set up the environment and explore the available features. If you have any questions or need further assistance, please refer to the documentation or reach out for support.