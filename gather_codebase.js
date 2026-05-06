const fs = require('fs');
const path = require('path');

const rootDir = __dirname;
const outputFile = path.join(rootDir, 'FullCodebase.txt');

const ignoredDirs = ['bin', 'obj', '.git', 'node_modules', 'wwwroot/uploads', '.antigravity', '.gemini', "frontend/velzon19Ang", "frontend/dist", ".angular"];
const allowedExtensions = ['.cs', '.json', '.csproj', '.sln', '.js', '.txt', '.md', '.ts', '.html', '.scss', '.css'];

function getFiles(dir, fileList = []) {
    const files = fs.readdirSync(dir);
    files.forEach(file => {
        const filePath = path.join(dir, file);
        const relativePath = path.relative(rootDir, filePath).split(path.sep).join('/');
        
        if (fs.statSync(filePath).isDirectory()) {
            if (!ignoredDirs.some(ignored => relativePath.startsWith(ignored) || file === ignored)) {
                getFiles(filePath, fileList);
            }
        } else {
            const ext = path.extname(file);
            if (allowedExtensions.includes(ext) && file !== 'FullCodebase.txt' && file !== 'ModelsCollection.txt') {
                fileList.push(filePath);
            }
        }
    });
    return fileList;
}

const allFiles = getFiles(rootDir);
let combinedContent = '';

allFiles.forEach(file => {
    const relativePath = path.relative(rootDir, file);
    const content = fs.readFileSync(file, 'utf8');
    combinedContent += `\n\n--- FILE: ${relativePath} ---\n\n`;
    combinedContent += content;
});

fs.writeFileSync(outputFile, combinedContent);
console.log(`Successfully gathered ${allFiles.length} files into ${outputFile}`);
