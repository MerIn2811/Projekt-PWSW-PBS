<?php
require __DIR__ . '/db.php';
require __DIR__ . '/helpers.php';


if (($_SERVER['REQUEST_METHOD'] ?? '') !== 'POST') {
    respond(405, ["error" => "Use POST"]);
}

if (!isset($_FILES['avatar']) || $_FILES['avatar']['error'] !== UPLOAD_ERR_OK) {
    respond(400, ["error" => "Missing avatar file"]);
}

$f = $_FILES['avatar'];

if ($f['size'] > 2 * 1024 * 1024) { // 2MB
    respond(400, ["error" => "File too large (max 2MB)"]);
}

$tmp = $f['tmp_name'];
$mime = mime_content_type($tmp);
$allowed = ['image/jpeg' => 'jpg', 'image/png' => 'png', 'image/webp' => 'webp'];

if (!isset($allowed[$mime])) {
    respond(400, ["error" => "Invalid file type. Use JPG/PNG/WEBP"]);
}

$ext = $allowed[$mime];

$filename = bin2hex(random_bytes(16)) . "." . $ext;

$dir = realpath(__DIR__ . '/../uploads');
if ($dir === false) {
    respond(500, ["error" => "Upload dir not found"]);
}

$dest = $dir . DIRECTORY_SEPARATOR . $filename;

if (!move_uploaded_file($tmp, $dest)) {
    respond(500, ["error" => "Failed to save file"]);
}

$avatarUrl = "https://lotekweronika.pl/uploads/" . $filename;

respond(200, [
    "ok" => true,
    "file" => $filename,
    "avatarUrl" => $avatarUrl
]);