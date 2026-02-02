<?php
require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';

$userId = require_auth($pdo);
$data = read_body_data();

$goalId = (int)($data['goalId'] ?? 0);
if ($goalId <= 0) respond(400, ["error" => "Missing goalId"]);

$name = trim((string)($data['name'] ?? ''));
$endDate = (string)($data['endDate'] ?? '');
$importance = (int)($data['importance'] ?? 0);
$description = (string)($data['description'] ?? '');

if ($name === '') respond(400, ["error" => "Missing name"]);
if ($endDate === '') respond(400, ["error" => "Missing dates"]);
if ($importance < 0 || $importance > 5) respond(400, ["error" => "Importance must be 0..5"]);

$endDate   = str_replace('T', ' ', $endDate);



$stmt = $pdo->prepare("
    INSERT INTO `task` (`idGoal`, `name`, `endDate`, `description`, `importance`, `isFinished`)
    VALUES (:g, :n, :e, :d, :i, 0)
    ");

$stmt->execute([
        "g" => $goalId,
        "n" => $name,
        "e" => $endDate,
        "i" => $importance,
        "d" => $description
    ]);

    $newId = (int)$pdo->lastInsertId();



respond(201, ["taskId" => $newId]);

?>