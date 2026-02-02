<?php
$host = "localhost";
$db   = "srv96435_PWSW_app";
$user = "srv96435_projekt";
$pass = "gQehGdVZNNtPTSVrPW4Y";
$charset = "utf8mb4";

$dsn = "mysql:host=$host;dbname=$db;charset=$charset";
try {
    $pdo = new PDO($dsn, $user, $pass, [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
        PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    ]);
} catch (PDOException $e) {
    http_response_code(500);
    echo json_encode(["error" => "DB error"]);
    exit;
}
?>