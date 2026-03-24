import pdfplumber, os

folder = r'c:\Users\Pateu\Desktop\Stuff\PID\CURS'
out_file = r'c:\Users\Pateu\Desktop\Stuff\PID\CURS\output.txt'
pdfs = sorted([f for f in os.listdir(folder) if f.endswith('.pdf')])

with open(out_file, 'w', encoding='utf-8') as out:
    for pdf_name in pdfs:
        path = os.path.join(folder, pdf_name)
        out.write('='*60 + '\n')
        out.write(f'FISIER: {pdf_name}\n')
        out.write('='*60 + '\n')
        try:
            with pdfplumber.open(path) as pdf:
                out.write(f'Numar pagini: {len(pdf.pages)}\n')
                for i, page in enumerate(pdf.pages):
                    try:
                        text = page.extract_text()
                        if text:
                            out.write(f'--- Pagina {i+1} ---\n')
                            out.write(text + '\n')
                    except Exception as e:
                        out.write(f'--- Pagina {i+1}: eroare ({e}) ---\n')
        except Exception as e:
            out.write(f'Eroare la deschidere: {e}\n')
        out.write('\n')

print("GATA!")
