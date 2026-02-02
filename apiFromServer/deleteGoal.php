<?php
require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';

$userId = require_auth($pdo);

$goalId = isset($_GET['goalId']) ? (int)$_GET['goalId'] : 0;
if ($goalId <= 0) {
    $raw = file_get_contents("php://input");
    $data = json_decode($raw, true);

    if (is_array($data) && isset($data['goalId'])) {
        $goalId = (int)$data['goalId'];
    }
}

if ($goalId <= 0) respond(400, ["error" => "Missing goalId"]);

$own = $pdo->prepare("SELECT 1 FROM goal WHERE idGoal = :gid AND userId = :uid LIMIT 1");
$own->execute(["gid" => $goalId, "uid" => $userId]);
if (!$own->fetch()) respond(403, ["error" => "Forbidden"]);

$stmt = $pdo->prepare("
    DELETE FROM goal WHERE idGoal = :gid
");
$stmt->execute(["gid" => $goalId]);


if ($stmt->rowCount() === 1) respond(200, ["task" => "ok!"]);
else respond(406, ["task" => "goal not deleted"]);


?>

