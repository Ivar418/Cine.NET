using NUnit.Framework;
using SharedLibrary.Domain.Entities;
using System;

namespace UnitTest.Domain.Entities;

[TestFixture]
[TestOf(typeof(User))]
public class UserTest {
    [Test]
    public void Constructor_ValidParameters_ShouldInitializeProperties() {
        // Arrange
        string userName = "testuser";
        string firstName = "John";
        string lastName = "Doe";
        string email = "john.doe@example.com";

        // Act
        var user = new User(userName, firstName, lastName, email);

        // Assert
        Assert.AreEqual(userName, user.UserName);
        Assert.AreEqual(firstName, user.FirstName);
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(email, user.Email);
        Assert.That(user.CreatedAt, Is.EqualTo(user.UpdatedAt));
    }

    [Test]
    public void ChangeName_ValidParameters_ShouldChangeNamesAndUpdateTimestamp() {
        // Arrange
        var user = new User("testuser", "John", "Doe", "john.doe@example.com");
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        var initialTimestamp = user.UpdatedAt;
        user.ChangeName(newFirstName, newLastName);

        // Assert
        Assert.AreEqual(newFirstName, user.FirstName);
        Assert.AreEqual(newLastName, user.LastName);
        Assert.That(initialTimestamp, Is.LessThan(user.UpdatedAt));
    }

    [Test]
    public void ChangeEmail_ValidEmail_ShouldChangeEmailAndUpdateTimestamp() {
        // Arrange
        var user = new User("testuser", "John", "Doe", "john.doe@example.com");
        var newEmail = "new.email@example.com";

        // Act
        var initialTimestamp = user.UpdatedAt;
        user.ChangeEmail(newEmail);

        // Assert
        Assert.AreEqual(newEmail, user.Email);
        Assert.That(initialTimestamp, Is.LessThan(user.UpdatedAt));
    }

    [Test]
    public void ChangeEmail_SameEmail_ShouldNotUpdateTimestamp() {
        // Arrange
        var user = new User("testuser", "John", "Doe", "john.doe@example.com");

        // Act
        var initialTimestamp = user.UpdatedAt;
        user.ChangeEmail("john.doe@example.com");

        // Assert
        Assert.AreEqual("john.doe@example.com", user.Email);
        Assert.That(initialTimestamp, Is.EqualTo(user.UpdatedAt));
    }

    [Test]
    public void ChangeUserName_ValidUsername_ShouldChangeUserNameAndUpdateTimestamp() {
        // Arrange
        var user = new User("testuser", "John", "Doe", "john.doe@example.com");
        var newUserName = "newuser";

        // Act
        var initialTimestamp = user.UpdatedAt;
        user.ChangeUserName(newUserName);

        // Assert
        Assert.AreEqual(newUserName, user.UserName);
        Assert.That(initialTimestamp, Is.LessThan(user.UpdatedAt));
    }

    [Test]
    public void ChangeUserName_SameUsername_ShouldNotUpdateTimestamp() {
        // Arrange
        var user = new User("testuser", "John", "Doe", "john.doe@example.com");

        // Act
        var initialTimestamp = user.UpdatedAt;
        user.ChangeUserName("testuser");

        // Assert
        Assert.AreEqual("testuser", user.UserName);
        Assert.That(initialTimestamp, Is.EqualTo(user.UpdatedAt));
    }

    [Test]
    public void ChangePhoto_ValidPhoto_ShouldUpdatePhotoAndUpdateTimestamp() {
        // Arrange
        var user = new User("testuser", "John", "Doe", "john.doe@example.com");
        var photo = new Photo
            { Id = "photo1", Url = "url", StorageKey = "key", Size = 123, ContentType = "image/png", EntityId = 1 };

        // Act
        var initialTimestamp = user.UpdatedAt;
        user.ChangePhoto(photo);

        // Assert
        Assert.AreEqual(photo, user.Photo);
        Assert.That(initialTimestamp, Is.LessThan(user.UpdatedAt));
    }

    [Test]
    public void ChangePhoto_SamePhoto_ShouldNotUpdateTimestamp() {
        // Arrange
        var photo = new Photo
            { Id = "photo1", Url = "url", StorageKey = "key", Size = 123, ContentType = "image/png", EntityId = 1 };
        var user = new User("testuser", "John", "Doe", "john.doe@example.com");
        user.ChangePhoto(photo);

        // Act
        var initialTimestamp = user.UpdatedAt;
        user.ChangePhoto(photo);

        // Assert
        Assert.AreEqual(photo, user.Photo);
        Assert.That(initialTimestamp, Is.EqualTo(user.UpdatedAt));
    }

    [Test]
    public void Constructor_InvalidUserName_ShouldThrowException() {
        Assert.Throws<ArgumentException>(() => new User("", "John", "Doe", "john.doe@example.com"));
        Assert.Throws<ArgumentException>(() => new User("  ", "John", "Doe", "john.doe@example.com"));
    }

    [Test]
    public void Constructor_InvalidFirstName_ShouldThrowException() {
        Assert.Throws<ArgumentException>(() => new User("testuser", "", "Doe", "john.doe@example.com"));
        Assert.Throws<ArgumentException>(() => new User("testuser", "  ", "Doe", "john.doe@example.com"));
    }

    [Test]
    public void Constructor_InvalidLastName_ShouldThrowException() {
        Assert.Throws<ArgumentException>(() => new User("testuser", "John", "", "john.doe@example.com"));
        Assert.Throws<ArgumentException>(() => new User("testuser", "John", "  ", "john.doe@example.com"));
    }

    [Test]
    public void Constructor_InvalidEmail_ShouldThrowException() {
        Assert.Throws<ArgumentException>(() => new User("testuser", "John", "Doe", ""));
        Assert.Throws<ArgumentException>(() => new User("testuser", "John", "Doe", "invalid-email"));
        Assert.Throws<ArgumentException>(() => new User("testuser", "John", "Doe", "   "));
    }
}