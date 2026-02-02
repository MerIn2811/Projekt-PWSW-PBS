<?php
require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';

$userId = require_auth($pdo);
$data = read_body_data();


$name = trim((string)($data['name'] ?? ''));
$endDate = (string)($data['endDate'] ?? '');
$category = (string)($data['category']);
$importance = (int)($data['importance'] ?? 0);
$description = (string)($data['description'] ?? '');

if ($name === '') respond(400, ["error" => "Missing name"]);
if ($endDate === '') respond(400, ["error" => "Missing dates"]);
if ($importance < 0 || $importance > 5) respond(400, ["error" => "Importance must be 0..5"]);

$endDate   = str_replace('T', ' ', $endDate);



$stmt = $pdo->prepare("
    INSERT INTO `goal` (`userId`, `name`, `endDate`, `category`, `importance`, `description`, `isFinished`)
    VALUES (:u, :n, :e, :c, :i, :d, 0)
    ");

$stmt->execute([
        "u" => $userId,
        "n" => $name,
        "e" => $endDate,
        "c" => $category,
        "i" => $importance,
        "d" => $description
    ]);

    $newId = (int)$pdo->lastInsertId();



respond(201, ["goalId" => $newId]);

?>