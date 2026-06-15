import os
import re
from fpdf import FPDF

class BeautifulPDF(FPDF):
    def header(self):
        # Header text
        self.set_font("Arial", "I", 9)
        self.set_text_color(100, 100, 100)
        # We use standard clean cell calling without deprecated ln parameter
        self.cell(0, 10, "LiverTransplantAR - Proje Dokümantasyonu", 0, 0, "R")
        self.ln(8)
        self.line(10, 17, 200, 17)
        self.ln(3)

    def footer(self):
        # Footer text
        self.set_y(-15)
        self.set_font("Arial", "I", 8)
        self.set_text_color(120, 120, 120)
        self.line(10, 280, 200, 280)
        page_num = f"Sayfa {self.page_no()}/{{nb}}"
        self.cell(0, 10, page_num, 0, 0, "C")

def clean_text(text):
    # Remove emojis and unsupported glyphs, but preserve Turkish characters
    # Preserve ASCII (0-127) and Turkish specific chars
    clean = ""
    for char in text:
        val = ord(char)
        # Allow ASCII and Latin Extended (Turkish characters are here)
        if val < 127 or (0x00C0 <= val <= 0x017F):
            clean += char
        else:
            # Replace common emojis with spaces or discard
            pass
    # Clean double spaces
    clean = re.sub(r'\s+', ' ', clean)
    return clean.strip()

def markdown_to_pdf(md_path, pdf_path):
    print(f"Derleniyor: {md_path} -> {pdf_path}")
    
    pdf = BeautifulPDF()
    pdf.alias_nb_pages()
    pdf.add_page()
    
    # Register Arial fonts for Turkish characters
    pdf.add_font("Arial", "", "/System/Library/Fonts/Supplemental/Arial.ttf")
    pdf.add_font("Arial", "B", "/System/Library/Fonts/Supplemental/Arial Bold.ttf")
    pdf.add_font("Arial", "I", "/System/Library/Fonts/Supplemental/Arial Italic.ttf")
    
    pdf.set_margins(15, 20, 15)
    pdf.set_auto_page_break(auto=True, margin=20)
    
    with open(md_path, "r", encoding="utf-8") as f:
        lines = f.readlines()
        
    for line in lines:
        line = line.strip()
        if not line:
            pdf.ln(3)
            continue
            
        # Parse Headers
        if line.startswith("# "):
            title_text = clean_text(line[2:])
            pdf.set_font("Arial", "B", 16)
            pdf.set_text_color(20, 80, 160) # Sleek blue
            pdf.ln(4)
            pdf.multi_cell(0, 8, title_text)
            pdf.ln(4)
        elif line.startswith("## "):
            subtitle_text = clean_text(line[3:])
            pdf.set_font("Arial", "B", 12)
            pdf.set_text_color(40, 40, 40)
            pdf.ln(3)
            pdf.multi_cell(0, 7, subtitle_text)
            pdf.ln(2)
        elif line.startswith("### "):
            sub_sub_text = clean_text(line[4:])
            pdf.set_font("Arial", "B", 10.5)
            pdf.set_text_color(60, 60, 60)
            pdf.ln(2)
            pdf.multi_cell(0, 6, sub_sub_text)
            pdf.ln(1)
        # Parse List Items
        elif line.startswith("- ") or line.startswith("* "):
            item_text = clean_text(line[2:])
            pdf.set_font("Arial", "", 10)
            pdf.set_text_color(50, 50, 50)
            
            # Bullet symbol and indent
            pdf.set_x(20)
            pdf.cell(5, 5.5, "-", 0, 0) # Standard dash instead of invalid dot character
            
            # Highlight bold markdown inside item (e.g. **Bold**)
            parts = re.split(r'(\*\*.*?\*\*)', item_text)
            for part in parts:
                if part.startswith("**") and part.endswith("**"):
                    pdf.set_font("Arial", "B", 10)
                    pdf.write(5.5, part[2:-2])
                else:
                    pdf.set_font("Arial", "", 10)
                    pdf.write(5.5, part)
            pdf.ln(5.5)
        # Parse Normal Paragraphs
        else:
            line_text = clean_text(line)
            pdf.set_font("Arial", "", 10)
            pdf.set_text_color(50, 50, 50)
            
            # Highlight bold markdown inside paragraph (e.g. **Bold**)
            parts = re.split(r'(\*\*.*?\*\*)', line_text)
            for part in parts:
                if part.startswith("**") and part.endswith("**"):
                    pdf.set_font("Arial", "B", 10)
                    pdf.write(6.5, part[2:-2])
                else:
                    pdf.set_font("Arial", "", 10)
                    pdf.write(6.5, part)
            pdf.ln(7.5)
            
    pdf.output(pdf_path)

if __name__ == "__main__":
    docs_dir = "docs"
    for file in os.listdir(docs_dir):
        if file.endswith(".md"):
            md_file = os.path.join(docs_dir, file)
            pdf_file = os.path.join(docs_dir, file.replace(".md", ".pdf"))
            markdown_to_pdf(md_file, pdf_file)
    print("Tüm dokümanlar başarıyla PDF formatına derlendi!")
