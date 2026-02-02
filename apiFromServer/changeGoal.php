<?php
require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';

$userId = require_auth($pdo);

$goalId = isset($_GET['goalId']) ? (int)$_GET['goalId'] : 0;
if ($goalId <= 0) respond(400, ["error" => "Missing goalId"]);

$own = $pdo->prepare("SELECT 1 FROM goal WHERE idGoal = :gid AND userId = :uid LIMIT 1");
$own->execute(["gid" => $goalId, "uid" => $userId]);
if (!$own->fetch()) respond(403, ["error" => "Forbidden"]);

$stmt = $pdo->prepare("
    SELECT idTask, idGoal, name, endDate, description, importance, isFinished
    FROM task
    WHERE idGoal = :gid
    ORDER BY isFinished ASC, endDate ASC
");
$stmt->execute(["gid" => $goalId]);

$rows = $stmt->fetchAll();

foreach ($rows as &$r){
    $r['isFinished'] = (bool)(int)$r['isFinished'];
    $r['importance'] = (int)$r['importance'];
    $r['idTask'] = (int)$r['idTask'];
    $r['idGoal'] = (int)$r['idGoal'];
}

unset($r);

respond(200, ["tasks" => $rows]);

?>