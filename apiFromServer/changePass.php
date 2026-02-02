<?php
declare(strict_types=1);

ini_set('log_errors', '1');
error_reporting(E_ALL);

require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';

try {
    if (($_SERVER['REQUEST_METHOD'] ?? '') !== 'POST') {
        respond(405, ["error" => "Use POST"]);
    }

    $userId = require_auth($pdo);

    $data = read_body_data();
    if (!is_array($data)) {
        respond(400, ["error" => "Invalid JSON"]);
    }

    $password = trim((string)($data['password'] ?? ($data['avatarUrl'] ?? '')));
    if ($avatar === '') {
        respond(400, ["error" => "Missing avatar"]);
    }

   

    $upd = $pdo->prepare("UPDATE `user` SET `password` = :p WHERE `idUser` = :uid");
    $upd->execute(["p" => $password, "uid" => $userId]);

    respond(200, ["ok" => true, "avatar" => $avatar, "rows" => $upd->rowCount()]);
}
catch (Throwable $e) {
    error_log("SET AVATAR ERROR: ".$e->getMessage()."\n".$e->getTraceAsString());
    http_response_code(500);
    header('Content-Type: application/json; charset=utf-8');
    echo json_encode(["error" => "Internal error", "details" => $e->getMessage()], JSON_UNESCAPED_UNICODE);
    exit;
}