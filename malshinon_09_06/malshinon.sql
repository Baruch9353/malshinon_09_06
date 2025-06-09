SET NAMES utf8mb4;


/* Create the database */
CREATE DATABASE IF NOT EXISTS malshinon;

/* Switch to the malshinon database */
USE malshinon;

/* Create People table */
CREATE TABLE People (
    id INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(250) NOT NULL,
    last_name VARCHAR(250) NOT NULL,
    secret_code VARCHAR(250) NOT NULL UNIQUE,
    type ENUM('reporter', 'target', 'both', 'potential_agent') NOT NULL,
    num_reports INT DEFAULT 0,
    num_mentions INT DEFAULT 0
);

/* Create IntelReports table */
CREATE TABLE IntelReports (
    id INT AUTO_INCREMENT PRIMARY KEY,
    reporter_id INT NOT NULL,
    target_id INT NOT NULL,
    text TEXT NOT NULL,
    timestamp DATETIME DEFAULT NOW(),
    FOREIGN KEY (reporter_id) REFERENCES People(id),
    FOREIGN KEY (target_id) REFERENCES People(id)
);
